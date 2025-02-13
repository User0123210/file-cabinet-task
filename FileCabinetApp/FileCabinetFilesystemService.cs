using FileCabinetApp.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        /// Creates a new value and adds it into the records list.
        /// </summary>
        /// <param name="recordParameters">Parameters of the value to change.</param>
        /// <returns>Id of the created value.</returns>
        public int CreateRecord(FileCabinetRecordParameterObject recordParameters)
        {
            if (recordParameters is not null)
            {
                Span<int> copyDecimal = new (new int[4]);
                decimal.GetBits(recordParameters.Salary, copyDecimal);
                byte[] value = new byte[RecordSize];
                this.stream.Position = 0;
                byte[] buffer = new byte[RecordSize];

                int newId = 1;
                List<FileCabinetRecord> records = this.GetRecords().ToList();

                while (records.Select(r => r.Id).Contains(newId))
                {
                    newId++;
                }

                this.stream.Seek(0, SeekOrigin.End);

                BitConverter.GetBytes(1).CopyTo(value, 0);
                BitConverter.GetBytes(newId).CopyTo(value, 2);
                Encoding.UTF8.GetBytes(recordParameters.FirstName.PadRight(120, '\0')).CopyTo(value, 6);
                Encoding.UTF8.GetBytes(recordParameters.LastName.PadRight(120, '\0')).CopyTo(value, 126);
                BitConverter.GetBytes(recordParameters.DateOfBirth.Year).CopyTo(value, 246);
                BitConverter.GetBytes(recordParameters.DateOfBirth.Month).CopyTo(value, 250);
                BitConverter.GetBytes(recordParameters.DateOfBirth.Day).CopyTo(value, 254);
                BitConverter.GetBytes(recordParameters.Status).CopyTo(value, 258);
                BitConverter.GetBytes(copyDecimal[0]).CopyTo(value, 274);
                BitConverter.GetBytes(copyDecimal[1]).CopyTo(value, 278);
                BitConverter.GetBytes(copyDecimal[2]).CopyTo(value, 282);
                BitConverter.GetBytes(copyDecimal[3]).CopyTo(value, 286);
                BitConverter.GetBytes(recordParameters.Permissions).CopyTo(value, 290);
                this.stream.Write(value, 0, RecordSize);
                this.stream.Flush();

                ulong recordIndex = (ulong)(this.stream.Position - RecordSize) / RecordSize;

                this.recordIdDictionary.Add(newId, recordIndex);

                AddToDictionary(this.firstNameDictionary, recordParameters.FirstName.ToUpperInvariant(), recordIndex);
                AddToDictionary(this.lastNameDictionary, recordParameters.LastName.ToUpperInvariant(), recordIndex);
                AddToDictionary(this.dateOfBirthDictionary, recordParameters.DateOfBirth, recordIndex);
                return newId;
            }

            return 0;
        }

        /// <summary>
        /// Gets copy of the records as value array.
        /// </summary>
        /// <returns>Array of the records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> records = new ();
            byte[] buffer = new byte[RecordSize];
            this.stream.Position = 0;
            int id;
            string firstName;
            string lastName;
            int year;
            int month;
            int day;
            short status;
            decimal salary;
            char permissions;
            int[] copyDecimal = new int[4];

            while (this.stream.Read(buffer, 0, RecordSize) != 0)
            {
                if ((buffer[0] & 4) != 0)
                {
                    continue;
                }

                id = BitConverter.ToInt32(buffer, 2);
                firstName = Encoding.UTF8.GetString(buffer[6..126]).TrimEnd('\0');
                lastName = Encoding.UTF8.GetString(buffer[126..246]).TrimEnd('\0');
                year = BitConverter.ToInt32(buffer, 246);
                month = BitConverter.ToInt32(buffer, 250);
                day = BitConverter.ToInt32(buffer, 254);
                DateTime dateOfBirth = new (year, month, day);
                status = BitConverter.ToInt16(buffer, 258);
                copyDecimal[0] = BitConverter.ToInt32(buffer, 274);
                copyDecimal[1] = BitConverter.ToInt32(buffer, 278);
                copyDecimal[2] = BitConverter.ToInt32(buffer, 282);
                copyDecimal[3] = BitConverter.ToInt32(buffer, 286);
                salary = new decimal(copyDecimal);
                permissions = BitConverter.ToChar(buffer, 290);

                ulong recordIndex = (ulong)(this.stream.Position - RecordSize) / RecordSize;

                if (!this.recordIdDictionary.ContainsKey(id))
                {
                    this.recordIdDictionary.Add(id, recordIndex);
                }

                if (!this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
                {
                    this.firstNameDictionary.Add(firstName.ToUpperInvariant(), new List<ulong>() { recordIndex });
                }
                else if (!this.firstNameDictionary[firstName.ToUpperInvariant()].Contains(recordIndex))
                {
                    this.firstNameDictionary[firstName.ToUpperInvariant()].Add(recordIndex);
                }

                if (!this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
                {
                    this.lastNameDictionary.Add(lastName.ToUpperInvariant(), new List<ulong>() { recordIndex });
                }
                else if (!this.lastNameDictionary[lastName.ToUpperInvariant()].Contains(recordIndex))
                {
                    this.lastNameDictionary[lastName.ToUpperInvariant()].Add(recordIndex);
                }

                if (!this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
                {
                    this.dateOfBirthDictionary.Add(dateOfBirth, new List<ulong>() { recordIndex });
                }
                else if (!this.dateOfBirthDictionary[dateOfBirth].Contains(recordIndex))
                {
                    this.dateOfBirthDictionary[dateOfBirth].Add(recordIndex);
                }

                records.Add(new FileCabinetRecord() { Id = id, FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status,  Salary = salary, Permissions = permissions });
            }

            this.stream.Position = 0;
            return records.AsReadOnly();
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
                byte[] buffer = new byte[RecordSize];
                this.stream.Position = 0;
                int recordId;

                Span<int> copyDecimal = new (new int[4]);
                decimal.GetBits(recordParameters.Salary, copyDecimal);
                byte[] value = new byte[RecordSize - 6];
                bool isFound = false;

                if (this.recordIdDictionary.ContainsKey(id))
                {
                    this.stream.Position = (long)(this.recordIdDictionary[id] * RecordSize);
                    this.stream.Read(buffer, 0, RecordSize);
                    isFound = EditIfFound(buffer, id, copyDecimal);
                }
                else
                {
                    while (this.stream.Read(buffer, 0, RecordSize) != 0)
                    {
                        isFound = EditIfFound(buffer, id, copyDecimal);

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

                bool EditIfFound(byte[] buffer, int id, Span<int> copyDecimal)
                {
                    recordId = BitConverter.ToInt32(buffer, 2);

                    if (recordId == id && ((buffer[0] & 4) == 0))
                    {
                        Encoding.UTF8.GetBytes(recordParameters.FirstName.PadRight(120, '\0')).CopyTo(value, 0);
                        Encoding.UTF8.GetBytes(recordParameters.LastName.PadRight(120, '\0')).CopyTo(value, 120);
                        BitConverter.GetBytes(recordParameters.DateOfBirth.Year).CopyTo(value, 240);
                        BitConverter.GetBytes(recordParameters.DateOfBirth.Month).CopyTo(value, 244);
                        BitConverter.GetBytes(recordParameters.DateOfBirth.Day).CopyTo(value, 248);
                        BitConverter.GetBytes(recordParameters.Status).CopyTo(value, 252);
                        BitConverter.GetBytes(copyDecimal[0]).CopyTo(value, 268);
                        BitConverter.GetBytes(copyDecimal[1]).CopyTo(value, 272);
                        BitConverter.GetBytes(copyDecimal[2]).CopyTo(value, 276);
                        BitConverter.GetBytes(copyDecimal[3]).CopyTo(value, 280);
                        BitConverter.GetBytes(recordParameters.Permissions).CopyTo(value, 284);
                        this.stream.Position -= RecordSize;
                        this.stream.Position += 6;
                        this.stream.Write(value, 0, RecordSize - 6);
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
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> records = new ();
            byte[] buffer = new byte[RecordSize];
            this.stream.Position = 0;
            int id;
            string recordFirstName;
            string lastName;
            int year;
            int month;
            int day;
            short status;
            decimal salary;
            char permissions;
            int[] copyDecimal = new int[4];
            bool isValid;

            if (firstName is not null)
            {
                if (this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
                {
                    for (int index = 0; index < this.firstNameDictionary[firstName.ToUpperInvariant()].Count; index++)
                    {
                        this.stream.Position = (long)(this.firstNameDictionary[firstName.ToUpperInvariant()][index] * RecordSize);
                        this.stream.Read(buffer, 0, RecordSize);
                        isValid = IfValidAddToRecords(buffer, firstName, ref records);

                        if (!isValid)
                        {
                            this.firstNameDictionary[firstName.ToUpperInvariant()].Remove(this.firstNameDictionary[firstName.ToUpperInvariant()][index]);
                        }
                    }
                }
                else
                {
                    while (this.stream.Read(buffer, 0, RecordSize) != 0)
                    {
                        isValid = IfValidAddToRecords(buffer, firstName!, ref records);

                        if (isValid)
                        {
                            ulong recordIndex = (ulong)(this.stream.Position - RecordSize) / RecordSize;
                            AddToDictionary(this.firstNameDictionary, firstName, recordIndex);
                        }
                    }
                }
            }

            return records.AsReadOnly();

            bool IfValidAddToRecords(byte[] buffer, string firstName, ref List<FileCabinetRecord> records)
            {
                recordFirstName = Encoding.UTF8.GetString(buffer[6..126]).TrimEnd('\0');

                if (string.Equals(recordFirstName, firstName, StringComparison.OrdinalIgnoreCase) && ((buffer[0] & 4) == 0))
                {
                    id = BitConverter.ToInt32(buffer, 2);
                    lastName = Encoding.UTF8.GetString(buffer[126..246]).TrimEnd('\0');
                    year = BitConverter.ToInt32(buffer, 246);
                    month = BitConverter.ToInt32(buffer, 250);
                    day = BitConverter.ToInt32(buffer, 254);
                    DateTime dateOfBirth = new (year, month, day);
                    status = BitConverter.ToInt16(buffer, 258);
                    copyDecimal[0] = BitConverter.ToInt32(buffer, 274);
                    copyDecimal[1] = BitConverter.ToInt32(buffer, 278);
                    copyDecimal[2] = BitConverter.ToInt32(buffer, 282);
                    copyDecimal[3] = BitConverter.ToInt32(buffer, 286);
                    salary = new decimal(copyDecimal);
                    permissions = BitConverter.ToChar(buffer, 290);
                    records.Add(new FileCabinetRecord() { Id = id, FirstName = recordFirstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status, Salary = salary, Permissions = permissions });
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Looks for records with lastName property equal to the specified lastName parameter.
        /// </summary>
        /// <param name="lastName">Last name of the records to seek.</param>
        /// <returns>Array of the found records with the specified lastName.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> records = new ();
            byte[] buffer = new byte[RecordSize];
            this.stream.Position = 0;
            int id;
            string firstName;
            string recordLastName;
            int year;
            int month;
            int day;
            short status;
            decimal salary;
            char permissions;
            int[] copyDecimal = new int[4];
            bool isValid;

            if (lastName is not null)
            {
                if (this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
                {
                    for (int index = 0; index < this.lastNameDictionary[lastName.ToUpperInvariant()].Count; index++)
                    {
                        this.stream.Position = (long)(this.lastNameDictionary[lastName.ToUpperInvariant()][index] * RecordSize);
                        this.stream.Read(buffer, 0, RecordSize);
                        isValid = IfValidAddToRecords(buffer, lastName, ref records);

                        if (!isValid)
                        {
                            this.lastNameDictionary[lastName.ToUpperInvariant()].Remove(this.lastNameDictionary[lastName.ToUpperInvariant()][index]);
                        }
                    }
                }
                else
                {
                    while (this.stream.Read(buffer, 0, RecordSize) != 0)
                    {
                        isValid = IfValidAddToRecords(buffer, lastName!, ref records);

                        if (isValid)
                        {
                            ulong recordIndex = (ulong)(this.stream.Position - RecordSize) / RecordSize;
                            AddToDictionary(this.firstNameDictionary, lastName, recordIndex);
                        }
                    }
                }
            }

            return records.AsReadOnly();

            bool IfValidAddToRecords(byte[] buffer, string lastName, ref List<FileCabinetRecord> records)
            {
                recordLastName = Encoding.UTF8.GetString(buffer[126..246]).TrimEnd('\0');

                if (string.Equals(recordLastName, lastName, StringComparison.OrdinalIgnoreCase) && ((buffer[0] & 4) == 0))
                {
                    id = BitConverter.ToInt32(buffer, 2);
                    firstName = Encoding.UTF8.GetString(buffer[6..126]).TrimEnd('\0');
                    year = BitConverter.ToInt32(buffer, 246);
                    month = BitConverter.ToInt32(buffer, 250);
                    day = BitConverter.ToInt32(buffer, 254);
                    DateTime dateOfBirth = new (year, month, day);
                    status = BitConverter.ToInt16(buffer, 258);
                    copyDecimal[0] = BitConverter.ToInt32(buffer, 274);
                    copyDecimal[1] = BitConverter.ToInt32(buffer, 278);
                    copyDecimal[2] = BitConverter.ToInt32(buffer, 282);
                    copyDecimal[3] = BitConverter.ToInt32(buffer, 286);
                    salary = new decimal(copyDecimal);
                    permissions = BitConverter.ToChar(buffer, 290);
                    records.Add(new FileCabinetRecord() { Id = id, FirstName = firstName, LastName = recordLastName, DateOfBirth = dateOfBirth, Status = status, Salary = salary, Permissions = permissions });
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Looks for records with dateOfBirth property equal to the specified date parameter.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth of the records to seek.</param>
        /// <returns>Array of the found records with the specified dateOfBirth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            List<FileCabinetRecord> records = new ();
            byte[] buffer = new byte[RecordSize];
            this.stream.Position = 0;
            int id;
            string firstName;
            string lastName;
            int year;
            int month;
            int day;
            short status;
            decimal salary;
            char permissions;
            int[] copyDecimal = new int[4];
            bool isValid;

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                for (int index = 0; index < this.dateOfBirthDictionary[dateOfBirth].Count; index++)
                {
                    this.stream.Position = (long)(this.dateOfBirthDictionary[dateOfBirth][index] * RecordSize);
                    this.stream.Read(buffer, 0, RecordSize);
                    isValid = IfValidAddToRecords(buffer, dateOfBirth, ref records);

                    if (!isValid)
                    {
                        this.dateOfBirthDictionary[dateOfBirth].Remove(this.dateOfBirthDictionary[dateOfBirth][index]);
                    }
                }
            }
            else
            {
                while (this.stream.Read(buffer, 0, RecordSize) != 0)
                {
                    isValid = IfValidAddToRecords(buffer, dateOfBirth!, ref records);

                    if (isValid)
                    {
                        ulong recordIndex = (ulong)(this.stream.Position - RecordSize) / RecordSize;
                        AddToDictionary(this.dateOfBirthDictionary, dateOfBirth, recordIndex);
                    }
                }
            }

            return records.AsReadOnly();

            bool IfValidAddToRecords(byte[] buffer, DateTime dateOfBirth, ref List<FileCabinetRecord> records)
            {
                year = BitConverter.ToInt32(buffer, 246);
                month = BitConverter.ToInt32(buffer, 250);
                day = BitConverter.ToInt32(buffer, 254);
                DateTime recordDateOfBirth = new (year, month, day);

                if (dateOfBirth == recordDateOfBirth && ((buffer[0] & 4) == 0))
                {
                    id = BitConverter.ToInt32(buffer, 2);
                    firstName = Encoding.UTF8.GetString(buffer[6..126]).TrimEnd('\0');
                    lastName = Encoding.UTF8.GetString(buffer[126..246]).TrimEnd('\0');
                    status = BitConverter.ToInt16(buffer, 258);
                    copyDecimal[0] = BitConverter.ToInt32(buffer, 274);
                    copyDecimal[1] = BitConverter.ToInt32(buffer, 278);
                    copyDecimal[2] = BitConverter.ToInt32(buffer, 282);
                    copyDecimal[3] = BitConverter.ToInt32(buffer, 286);
                    salary = new decimal(copyDecimal);
                    permissions = BitConverter.ToChar(buffer, 290);
                    records.Add(new FileCabinetRecord() { Id = id, FirstName = firstName, LastName = lastName, DateOfBirth = recordDateOfBirth, Status = status, Salary = salary, Permissions = permissions });
                    return true;
                }

                return false;
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
        /// Compares data from snapshot and updates records.
        /// </summary>
        /// <param name="snapshot">Snapshot to compare with.</param>
        /// <exception cref="NotImplementedException">Throws.</exception>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is not null)
            {
                List<FileCabinetRecord> newRecords = snapshot.Records.ToList();
                int[] existingIds = this.GetRecords().Select(r => r.Id).ToArray();

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
            int id;
            string firstName;
            string lastName;
            int year;
            int month;
            int day;
            short status;
            decimal salary;
            char permissions;
            int[] copyDecimal = new int[4];
            byte[] recordStatus;

            while (this.stream.Read(buffer, 0, RecordSize) != 0)
            {
                id = BitConverter.ToInt32(buffer, 2);

                if ((buffer[0] & 4) != 0)
                {
                    continue;
                }

                recordStatus = buffer[0..2];
                firstName = Encoding.UTF8.GetString(buffer[6..126]).TrimEnd('\0');
                lastName = Encoding.UTF8.GetString(buffer[126..246]).TrimEnd('\0');
                year = BitConverter.ToInt32(buffer, 246);
                month = BitConverter.ToInt32(buffer, 250);
                day = BitConverter.ToInt32(buffer, 254);
                DateTime dateOfBirth = new (year, month, day);
                status = BitConverter.ToInt16(buffer, 258);
                copyDecimal[0] = BitConverter.ToInt32(buffer, 274);
                copyDecimal[1] = BitConverter.ToInt32(buffer, 278);
                copyDecimal[2] = BitConverter.ToInt32(buffer, 282);
                copyDecimal[3] = BitConverter.ToInt32(buffer, 286);
                salary = new decimal(copyDecimal);
                permissions = BitConverter.ToChar(buffer, 290);

                this.firstNameDictionary.Remove(firstName.ToUpperInvariant());
                this.lastNameDictionary.Remove(lastName.ToUpperInvariant());
                this.dateOfBirthDictionary.Remove(dateOfBirth);

                records.Add((recordStatus, new FileCabinetRecord() { Id = id, FirstName = firstName, LastName = lastName, DateOfBirth = dateOfBirth, Status = status, Salary = salary, Permissions = permissions }));
            }

            this.stream.Position = 0;

            foreach (var record in records)
            {
                BitConverter.GetBytes(record.Item1[0]).CopyTo(value, 0);
                BitConverter.GetBytes(record.Item1[1]).CopyTo(value, 1);
                BitConverter.GetBytes(record.Item2.Id).CopyTo(value, 2);
                Encoding.UTF8.GetBytes(record.Item2.FirstName.PadRight(120, '\0')).CopyTo(value, 6);
                Encoding.UTF8.GetBytes(record.Item2.LastName.PadRight(120, '\0')).CopyTo(value, 126);
                BitConverter.GetBytes(record.Item2.DateOfBirth.Year).CopyTo(value, 246);
                BitConverter.GetBytes(record.Item2.DateOfBirth.Month).CopyTo(value, 250);
                BitConverter.GetBytes(record.Item2.DateOfBirth.Day).CopyTo(value, 254);
                BitConverter.GetBytes(record.Item2.Status).CopyTo(value, 258);
                decimal.GetBits(record.Item2.Salary, copyDecimal);
                BitConverter.GetBytes(copyDecimal[0]).CopyTo(value, 274);
                BitConverter.GetBytes(copyDecimal[1]).CopyTo(value, 278);
                BitConverter.GetBytes(copyDecimal[2]).CopyTo(value, 282);
                BitConverter.GetBytes(copyDecimal[3]).CopyTo(value, 286);
                BitConverter.GetBytes(record.Item2.Permissions).CopyTo(value, 290);
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
                targetDictionary[key].Add(position);
            }
            else
            {
                targetDictionary.Add(key, new List<ulong>() { position });
            }
        }
    }
}
