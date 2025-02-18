using FileCabinetApp.Validators;
using System.Collections.Immutable;
using System.Text;

#pragma warning disable SA1011

namespace FileCabinetApp
{
    /// <summary>
    /// Represent file cabinet service for work with filesytem.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordSize = 292;
        private readonly FileStream stream;
        private IRecordValidator validator;
        private readonly SortedDictionary<string, List<ulong>> firstNameDictionary = new ();
        private readonly SortedDictionary<string, List<ulong>> lastNameDictionary = new ();
        private readonly SortedDictionary<DateTime, List<ulong>> dateOfBirthDictionary = new ();
        private readonly SortedDictionary<int, ulong> recordIdDictionary = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="stream">Stream to write.</param>
        /// <param name="validator">Validator to check record parameters.</param>
        public FileCabinetFilesystemService(FileStream stream, IRecordValidator validator)
        {
            this.stream = stream;
            this.validator = validator;

            foreach (var record in this.GetRecords())
            {
            }
        }

        /// <summary>
        /// Gets a value of the date format.
        /// </summary>
        /// <value>dateFormat.</value>
        public string DateFormat
        {
            get
            {
                CompositeValidator? compositeValidator = this.validator as CompositeValidator;

                if (compositeValidator is not null)
                {
                    foreach (var validator in compositeValidator.GetValidators())
                    {
                        if (validator.GetType() == typeof(DateOfBirthValidator))
                        {
                            DateOfBirthValidator? dateOfBirthValidator = validator as DateOfBirthValidator;

                            if (dateOfBirthValidator is not null)
                            {
                                return dateOfBirthValidator.DateFormat;
                            }
                        }
                    }
                }

                return "MM/dd/yyyy";
            }
        }

        /// <summary>
        /// Gets array of validators to validate records in the service.
        /// </summary>
        /// <returns>Array of validators.</returns>
        public IRecordValidator[]? GetValidators()
        {
            CompositeValidator? compositeValidator = this.validator as CompositeValidator;

            if (compositeValidator is not null)
            {
                return compositeValidator.GetValidators();
            }

            return null;
        }

        /// <summary>
        /// Gets information about the number of records in the service.
        /// </summary>
        /// <value>
        /// <records.Count>Information about the number of records in the service.</records.Count>
        /// </value>
        public (int, int) GetStat
        {
            get
            {
                int numberOfDeleted = 0;
                int numberOfRecords = 0;
                this.stream.Position = 0;
                byte[] buffer = new byte[RecordSize];

                while (this.stream.Read(buffer, 0, RecordSize) != 0)
                {
                    if ((buffer[0] & 4) != 0)
                    {
                        numberOfDeleted++;
                    }

                    numberOfRecords++;
                }

                return (numberOfRecords, numberOfDeleted);
            }
        }

        /// <summary>
        /// Gets cached search data
        /// </summary>
        /// <value>Copy of the search cache dictionary.</value>
        public Dictionary<ImmutableArray<(string, string)>, IReadOnlyCollection<FileCabinetRecord>> SearchCache
        {
            get => new ();
        }

        /// <summary>
        /// Creates a new value and adds it into the records list.
        /// </summary>
        /// <param name="recordParameters">Parameters of the record to add.</param>
        /// <returns>Id of the created value.</returns>
        public int CreateRecord(FileCabinetRecordParameterObject recordParameters)
        {
            int newId = 1;

            while (this.recordIdDictionary.ContainsKey(newId))
            {
                newId++;
            }

            return this.CreateRecord(newId, recordParameters);
        }

        /// <summary>
        /// Creates a new value and adds it into the records list.
        /// </summary>
        /// <param name="id">Parameters of the record to add.</param>
        /// <param name="recordParameters">Parameters of the record to add.</param>
        /// <returns>Id of the created value.</returns>
        public int CreateRecord(int id, FileCabinetRecordParameterObject recordParameters)
        {
            if (recordParameters is not null)
            {
                ArgumentNullException.ThrowIfNull(recordParameters);

                (bool isValid, string message) = this.validator.ValidateParameters(recordParameters);

                if (!isValid)
                {
                    throw new ArgumentException(message);
                }

                Span<int> copyDecimal = new (new int[4]);
                decimal.GetBits(recordParameters.Salary, copyDecimal);
                byte[] value = new byte[RecordSize];
                this.stream.Position = 0;

                this.stream.Seek(0, SeekOrigin.End);

                if (this.recordIdDictionary.ContainsKey(id))
                {
                    throw new ArgumentException("Record with this id already exists.");
                }

                BitConverter.GetBytes(1).CopyTo(value, 0);
                BitConverter.GetBytes(id).CopyTo(value, 2);
                GetBytes(recordParameters, ref value);
                this.stream.Write(value, 0, RecordSize);
                this.stream.Flush();

                ulong recordIndex = (ulong)(this.stream.Position - RecordSize) / RecordSize;

                this.recordIdDictionary.Add(id, recordIndex);

                AddToDictionary(this.firstNameDictionary, recordParameters.FirstName.ToUpperInvariant(), recordIndex);
                AddToDictionary(this.lastNameDictionary, recordParameters.LastName.ToUpperInvariant(), recordIndex);
                AddToDictionary(this.dateOfBirthDictionary, recordParameters.DateOfBirth, recordIndex);
                return id;
            }

            return 0;
        }

        /// <summary>
        /// Gets copy of the records as value array.
        /// </summary>
        /// <returns>Array of the records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            byte[] buffer = new byte[RecordSize];
            this.stream.Position = 0;

            while (this.stream.Read(buffer, 0, RecordSize) != 0)
            {
                if ((buffer[0] & 4) != 0)
                {
                    continue;
                }

                FileCabinetRecord record = GetRecordFromBytes(buffer);

                ulong recordIndex = (ulong)(this.stream.Position - RecordSize) / RecordSize;

                if (!this.recordIdDictionary.ContainsKey(record.Id))
                {
                    this.recordIdDictionary.Add(record.Id, recordIndex);
                }

                AddToDictionary(this.firstNameDictionary, record.FirstName.ToUpperInvariant(), recordIndex);
                AddToDictionary(this.lastNameDictionary, record.LastName.ToUpperInvariant(), recordIndex);
                AddToDictionary(this.dateOfBirthDictionary, record.DateOfBirth, recordIndex);

                yield return record;
            }

            this.stream.Position = 0;
        }

        /// <summary>
        /// Edits the existing value with the specified id.
        /// </summary>
        /// <param name="id">Id of the value to edit.</param>
        /// <param name="recordParameters">Parameters of the value to change.</param>
        public void EditRecord(int id, FileCabinetRecordParameterObject recordParameters)
        {
            if (recordParameters is not null)
            {
                ArgumentNullException.ThrowIfNull(recordParameters);

                (bool isValid, string message) = this.validator.ValidateParameters(recordParameters);

                if (!isValid)
                {
                    throw new ArgumentException(message);
                }

                byte[] buffer = new byte[RecordSize];
                this.stream.Position = 0;
                int recordId;

                byte[] value = new byte[RecordSize];
                bool isFound = false;

                if (this.recordIdDictionary.ContainsKey(id))
                {
                    this.stream.Position = (long)(this.recordIdDictionary[id] * RecordSize);
                    this.stream.Read(buffer, 0, RecordSize);
                    isFound = EditIfFound(buffer, id);
                }
                else
                {
                    while (this.stream.Read(buffer, 0, RecordSize) != 0)
                    {
                        isFound = EditIfFound(buffer, id);

                        if (isFound)
                        {
                            break;
                        }
                    }
                }

                if (!isFound)
                {
                    throw new ArgumentException("Record is not found");
                }

                bool EditIfFound(byte[] buffer, int id)
                {
                    recordId = BitConverter.ToInt32(buffer, 2);

                    if (recordId == id && ((buffer[0] & 4) == 0))
                    {
                        GetBytes(recordParameters, ref value);
                        this.stream.Position -= RecordSize;
                        this.stream.Position += 6;
                        this.stream.Write(value[6..], 0, RecordSize - 6);
                        this.stream.Flush();

                        string firstName = Encoding.UTF8.GetString(buffer[6..126]).TrimEnd('\0');
                        string lastName = Encoding.UTF8.GetString(buffer[126..246]).TrimEnd('\0');
                        int year = BitConverter.ToInt32(buffer, 246);
                        int month = BitConverter.ToInt32(buffer, 250);
                        int day = BitConverter.ToInt32(buffer, 254);
                        DateTime dateOfBirth = new (year, month, day);

                        ulong recordIndex = (ulong)(this.stream.Position - RecordSize) / RecordSize;

                        if (this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
                        {
                            this.firstNameDictionary[firstName.ToUpperInvariant()].Remove(recordIndex);
                        }

                        if (this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
                        {
                             this.lastNameDictionary[lastName.ToUpperInvariant()].Remove(recordIndex);
                        }

                        if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
                        {
                            this.dateOfBirthDictionary[dateOfBirth].Remove(recordIndex);
                        }

                        AddToDictionary(this.firstNameDictionary, recordParameters.FirstName.ToUpperInvariant(), recordIndex);
                        AddToDictionary(this.lastNameDictionary, recordParameters.LastName.ToUpperInvariant(), recordIndex);
                        AddToDictionary(this.dateOfBirthDictionary, recordParameters.DateOfBirth, recordIndex);

                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Looks for records with firstName property equal to the specified firstName parameter.
        /// </summary>
        /// <param name="firstName">First name of the records to seek.</param>
        /// <returns>Array of the found records with the specified firstName.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            byte[] buffer = new byte[RecordSize];
            this.stream.Position = 0;
            bool isValid;
            FileCabinetRecord rec;

            if (firstName is not null)
            {
                if (this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
                {
                    for (int index = 0; index < this.firstNameDictionary[firstName.ToUpperInvariant()].Count; index++)
                    {
                        this.stream.Position = (long)(this.firstNameDictionary[firstName.ToUpperInvariant()][index] * RecordSize);
                        this.stream.Read(buffer, 0, RecordSize);
                        (isValid, rec) = IfValidAddToRecords(buffer, firstName);

                        if (!isValid)
                        {
                            this.firstNameDictionary[firstName.ToUpperInvariant()].Remove(this.firstNameDictionary[firstName.ToUpperInvariant()][index]);
                        }
                        else
                        {
                            yield return rec;
                        }
                    }
                }
                else
                {
                    while (this.stream.Read(buffer, 0, RecordSize) != 0)
                    {
                        (isValid, rec) = IfValidAddToRecords(buffer, firstName!);

                        if (isValid)
                        {
                            ulong recordIndex = (ulong)(this.stream.Position - RecordSize) / RecordSize;
                            AddToDictionary(this.firstNameDictionary, firstName.ToUpperInvariant(), recordIndex);
                            yield return rec;
                        }
                    }
                }
            }

            static (bool, FileCabinetRecord) IfValidAddToRecords(byte[] buffer, string firstName)
            {
                string recordFirstName = Encoding.UTF8.GetString(buffer[6..126]).TrimEnd('\0');

                if (string.Equals(recordFirstName, firstName, StringComparison.OrdinalIgnoreCase) && ((buffer[0] & 4) == 0))
                {
                    FileCabinetRecord rec = GetRecordFromBytes(buffer);
                    return (true, rec);
                }

                return (false, new FileCabinetRecord());
            }
        }

        /// <summary>
        /// Looks for records with lastName property equal to the specified lastName parameter.
        /// </summary>
        /// <param name="lastName">Last name of the records to seek.</param>
        /// <returns>Array of the found records with the specified lastName.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            byte[] buffer = new byte[RecordSize];
            this.stream.Position = 0;
            bool isValid;
            FileCabinetRecord rec;

            if (lastName is not null)
            {
                if (this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
                {
                    for (int index = 0; index < this.lastNameDictionary[lastName.ToUpperInvariant()].Count; index++)
                    {
                        this.stream.Position = (long)(this.lastNameDictionary[lastName.ToUpperInvariant()][index] * RecordSize);
                        this.stream.Read(buffer, 0, RecordSize);
                        (isValid, rec) = IfValidAddToRecords(buffer, lastName);

                        if (!isValid)
                        {
                            this.lastNameDictionary[lastName.ToUpperInvariant()].Remove(this.lastNameDictionary[lastName.ToUpperInvariant()][index]);
                        }
                        else
                        {
                            yield return rec;
                        }
                    }
                }
                else
                {
                    while (this.stream.Read(buffer, 0, RecordSize) != 0)
                    {
                        (isValid, rec) = IfValidAddToRecords(buffer, lastName!);

                        if (isValid)
                        {
                            ulong recordIndex = (ulong)(this.stream.Position - RecordSize) / RecordSize;
                            AddToDictionary(this.lastNameDictionary, lastName.ToUpperInvariant(), recordIndex);
                            yield return rec;
                        }
                    }
                }
            }

            static (bool, FileCabinetRecord) IfValidAddToRecords(byte[] buffer, string lastName)
            {
                string recordLastName = Encoding.UTF8.GetString(buffer[126..246]).TrimEnd('\0');

                if (string.Equals(recordLastName, lastName, StringComparison.OrdinalIgnoreCase) && ((buffer[0] & 4) == 0))
                {
                    FileCabinetRecord rec = GetRecordFromBytes(buffer);
                    return (true, rec);
                }

                return (false, new FileCabinetRecord());
            }
        }

        /// <summary>
        /// Looks for records with dateOfBirth property equal to the specified date parameter.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth of the records to seek.</param>
        /// <returns>Array of the found records with the specified dateOfBirth.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            byte[] buffer = new byte[RecordSize];
            this.stream.Position = 0;
            bool isValid;
            FileCabinetRecord rec;

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                for (int index = 0; index < this.dateOfBirthDictionary[dateOfBirth].Count; index++)
                {
                    this.stream.Position = (long)(this.dateOfBirthDictionary[dateOfBirth][index] * RecordSize);
                    this.stream.Read(buffer, 0, RecordSize);
                    (isValid, rec) = IfValidAddToRecords(buffer, dateOfBirth);

                    if (!isValid)
                    {
                        this.dateOfBirthDictionary[dateOfBirth].Remove(this.dateOfBirthDictionary[dateOfBirth][index]);
                    }
                    else
                    {
                        yield return rec;
                    }
                }
            }
            else
            {
                while (this.stream.Read(buffer, 0, RecordSize) != 0)
                {
                    (isValid, rec) = IfValidAddToRecords(buffer, dateOfBirth!);

                    if (isValid)
                    {
                        ulong recordIndex = (ulong)(this.stream.Position - RecordSize) / RecordSize;
                        AddToDictionary(this.dateOfBirthDictionary, dateOfBirth, recordIndex);
                        yield return rec;
                    }
                }
            }

            static (bool, FileCabinetRecord) IfValidAddToRecords(byte[] buffer, DateTime dateOfBirth)
            {
                int year = BitConverter.ToInt32(buffer, 246);
                int month = BitConverter.ToInt32(buffer, 250);
                int day = BitConverter.ToInt32(buffer, 254);
                DateTime recordDateOfBirth = new (year, month, day);

                if (dateOfBirth == recordDateOfBirth && ((buffer[0] & 4) == 0))
                {
                    FileCabinetRecord rec = GetRecordFromBytes(buffer);
                    return (true, rec);
                }

                return (false, new FileCabinetRecord());
            }
        }

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetDefaultService.
        /// </summary>
        public void ChangeValidatorToCustom() => this.validator = new ValidatorBuilder().CreateCustom();

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetCustomService.
        /// </summary>
        public void ChangeValidatorToDefault() => this.validator = new ValidatorBuilder().CreateDefault();

        /// <summary>
        /// Makes snapshot of the IFileCabinetService with the copy of the records.
        /// </summary>
        /// <returns>FileCabinetServiceSnapshot of the current IFileCabinetService instance.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.GetRecords());
        }

        /// <summary>
        /// Adds cache data to the cache.
        /// </summary>
        public void AddToSearchCache(ImmutableArray<(string, string)> criteria, IReadOnlyCollection<FileCabinetRecord> data)
        {
        }

        /// <summary>
        /// Compares data from snapshot and updates records.
        /// </summary>
        /// <param name="snapshot">Snapshot to compare with.</param>
        /// <exception cref="NotImplementedException">Throws.</exception>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is not null)
            {
                List<FileCabinetRecord> newRecords = snapshot.Records.ToList();
                int[] existingIds = this.recordIdDictionary.Select(v => v.Key).ToArray();

                foreach (var rec in newRecords)
                {
                    Tuple<bool, string> validationResult = this.validator.ValidateParameters(new FileCabinetRecordParameterObject() { FirstName = rec.FirstName, LastName = rec.LastName, DateOfBirth = rec.DateOfBirth, Status = rec.Status, Salary = rec.Salary, Permissions = rec.Permissions });

                    if (!validationResult.Item1)
                    {
                        Console.WriteLine($"Record #{rec.Id}, {validationResult.Item2}, skips.");
                        continue;
                    }

                    if (existingIds.Contains(rec.Id))
                    {
                        this.EditRecord(rec.Id, new FileCabinetRecordParameterObject() { FirstName = rec.FirstName, LastName = rec.LastName, DateOfBirth = rec.DateOfBirth, Status = rec.Status, Salary = rec.Salary, Permissions = rec.Permissions });
                    }
                    else
                    {
                        int id = this.CreateRecord(new FileCabinetRecordParameterObject() { FirstName = rec.FirstName, LastName = rec.LastName, DateOfBirth = rec.DateOfBirth, Status = rec.Status, Salary = rec.Salary, Permissions = rec.Permissions });
                        this.recordIdDictionary.Remove(id);
                        this.stream.Position -= RecordSize;
                        this.recordIdDictionary.Add(rec.Id, (ulong)this.stream.Position / RecordSize);
                        this.stream.Position += 2;
                        this.stream.Write(BitConverter.GetBytes(rec.Id), 0, 4);
                        this.stream.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Removes record with the specified id.
        /// </summary>
        /// <param name="id">Id of the record to delete.</param>
        public void RemoveRecord(int id)
        {
            byte[] buffer = new byte[RecordSize];
            this.stream.Position = 0;
            int recordId;
            byte[] value;

            if (this.recordIdDictionary.ContainsKey(id))
            {
                this.stream.Position = (long)(this.recordIdDictionary[id] * RecordSize);
                this.stream.Read(buffer, 0, RecordSize);
                IsExistRemove(buffer, id);
            }
            else
            {
                while (this.stream.Read(buffer, 0, RecordSize) != 0)
                {
                    IsExistRemove(buffer, id);
                }
            }

            void IsExistRemove(byte[] buffer, int id)
            {
                recordId = BitConverter.ToInt32(buffer, 2);

                if (recordId == id)
                {
                    value = buffer[0..2];
                    value[0] |= 4;
                    this.stream.Position -= RecordSize;
                    this.stream.Write(value, 0, 2);
                    this.stream.Flush();
                    string firstName = Encoding.UTF8.GetString(buffer[6..126]).TrimEnd('\0');
                    string lastName = Encoding.UTF8.GetString(buffer[126..246]).TrimEnd('\0');
                    int year = BitConverter.ToInt32(buffer, 246);
                    int month = BitConverter.ToInt32(buffer, 250);
                    int day = BitConverter.ToInt32(buffer, 254);
                    DateTime dateOfBirth = new (year, month, day);

                    this.recordIdDictionary.Remove(id);

                    if (this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
                    {
                        this.firstNameDictionary[firstName.ToUpperInvariant()].Remove((ulong)(this.stream.Position - 2) / RecordSize);
                    }

                    if (this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
                    {
                        this.lastNameDictionary[lastName.ToUpperInvariant()].Remove((ulong)(this.stream.Position - 2) / RecordSize);
                    }

                    if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
                    {
                        this.dateOfBirthDictionary[dateOfBirth].Remove((ulong)(this.stream.Position - 2) / RecordSize);
                    }
                }
            }
        }

        /// <summary>
        /// Removes deleted records from source database.
        /// </summary>
        public void Purge()
        {
            List<(byte[], FileCabinetRecord)> records = new ();
            this.stream.Position = 0;
            byte[] value = new byte[RecordSize];
            byte[] buffer = new byte[RecordSize];
            byte[] recordStatus;

            while (this.stream.Read(buffer, 0, RecordSize) != 0)
            {
                if ((buffer[0] & 4) != 0)
                {
                    continue;
                }

                recordStatus = buffer[0..2];
                FileCabinetRecord rec = GetRecordFromBytes(buffer);

                this.firstNameDictionary.Remove(rec.FirstName.ToUpperInvariant());
                this.lastNameDictionary.Remove(rec.LastName.ToUpperInvariant());
                this.dateOfBirthDictionary.Remove(rec.DateOfBirth);

                records.Add((recordStatus, rec));
            }

            this.stream.Position = 0;

            foreach (var record in records)
            {
                BitConverter.GetBytes(record.Item1[0]).CopyTo(value, 0);
                BitConverter.GetBytes(record.Item1[1]).CopyTo(value, 1);
                BitConverter.GetBytes(record.Item2.Id).CopyTo(value, 2);
                FileCabinetRecordParameterObject parameters = new () { FirstName = record.Item2.FirstName, LastName = record.Item2.LastName, DateOfBirth = record.Item2.DateOfBirth, Status = record.Item2.Status, Salary = record.Item2.Salary, Permissions = record.Item2.Permissions };
                GetBytes(parameters, ref value);
                this.stream.Write(value, 0, RecordSize);
                this.stream.Flush(true);

                ulong recordIndex = (ulong)(this.stream.Position - RecordSize) / RecordSize;

                if (this.recordIdDictionary.ContainsKey(record.Item2.Id))
                {
                    this.recordIdDictionary[record.Item2.Id] = recordIndex;
                }
                else
                {
                    this.recordIdDictionary.Add(record.Item2.Id, recordIndex);
                }

                AddToDictionary(this.firstNameDictionary, record.Item2.FirstName.ToUpperInvariant(), recordIndex);
                AddToDictionary(this.lastNameDictionary, record.Item2.LastName.ToUpperInvariant(), recordIndex);
                AddToDictionary(this.dateOfBirthDictionary, record.Item2.DateOfBirth, recordIndex);
            }

            this.stream.Position = 0;
            this.stream.SetLength(RecordSize * records.Count);
            this.stream.Flush(false);
        }

        private static void AddToDictionary<T>(SortedDictionary<T, List<ulong>> targetDictionary, T key, ulong position)
            where T : notnull
        {
            if (targetDictionary.ContainsKey(key))
            {
                if (!targetDictionary[key].Contains(position))
                {
                    targetDictionary[key].Add(position);
                }
            }
            else
            {
                targetDictionary.Add(key, new List<ulong>() { position });
            }
        }

        private static void GetBytes(FileCabinetRecordParameterObject record, ref byte[] value)
        {
                int[] copyDecimal = new int[4];
                Encoding.UTF8.GetBytes(record.FirstName.PadRight(120, '\0')).CopyTo(value, 6);
                Encoding.UTF8.GetBytes(record.LastName.PadRight(120, '\0')).CopyTo(value, 126);
                BitConverter.GetBytes(record.DateOfBirth.Year).CopyTo(value, 246);
                BitConverter.GetBytes(record.DateOfBirth.Month).CopyTo(value, 250);
                BitConverter.GetBytes(record.DateOfBirth.Day).CopyTo(value, 254);
                BitConverter.GetBytes(record.Status).CopyTo(value, 258);
                decimal.GetBits(record.Salary, copyDecimal);
                BitConverter.GetBytes(copyDecimal[0]).CopyTo(value, 274);
                BitConverter.GetBytes(copyDecimal[1]).CopyTo(value, 278);
                BitConverter.GetBytes(copyDecimal[2]).CopyTo(value, 282);
                BitConverter.GetBytes(copyDecimal[3]).CopyTo(value, 286);
                BitConverter.GetBytes(record.Permissions).CopyTo(value, 290);
        }

        private static FileCabinetRecord GetRecordFromBytes(byte[] buffer)
        {
            int year;
            int month;
            int day;
            int[] copyDecimal = new int[4];
            int id = BitConverter.ToInt32(buffer, 2);
            string firstName = Encoding.UTF8.GetString(buffer[6..126]).TrimEnd('\0');
            string lastName = Encoding.UTF8.GetString(buffer[126..246]).TrimEnd('\0');
            year = BitConverter.ToInt32(buffer, 246);
            month = BitConverter.ToInt32(buffer, 250);
            day = BitConverter.ToInt32(buffer, 254);
            DateTime dateOfBirth = new (year, month, day);
            short status = BitConverter.ToInt16(buffer, 258);
            copyDecimal[0] = BitConverter.ToInt32(buffer, 274);
            copyDecimal[1] = BitConverter.ToInt32(buffer, 278);
            copyDecimal[2] = BitConverter.ToInt32(buffer, 282);
            copyDecimal[3] = BitConverter.ToInt32(buffer, 286);
            decimal salary = new (copyDecimal);
            char permissions = BitConverter.ToChar(buffer, 290);
            return new FileCabinetRecord() { Id = id, FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status,  Salary = salary, Permissions = permissions };
        }
    }
}
