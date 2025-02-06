using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Manages information about the records in file cabinet.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> records = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();
        private readonly Dictionary<int, FileCabinetRecord> recordIdDictionary = new ();
        private IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Parameter of validator to use.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.records = new List<FileCabinetRecord>();
            this.validator = validator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Parameter of validator to use.</param>
        /// <param name="records">Parameter to assign to records list.</param>
        /// <param name="firstNameDictionary">Parameter to assign to firstNameDictionary dictionary.</param>
        /// <param name="lastNameDictionary">Parameter to assign to lastNameDictionary dictionary.</param>
        /// <param name="dateOfBirthDictionary">Parameter to assign to dateOfBirthDictionary dictionary.</param>
        /// <param name="recordIdDictionary">Parameter to assign to recordIdDictionary dictionary.</param>
        public FileCabinetMemoryService(IRecordValidator validator, IList<FileCabinetRecord> records, Dictionary<string, List<FileCabinetRecord>> firstNameDictionary, Dictionary<string, List<FileCabinetRecord>> lastNameDictionary, Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary, Dictionary<int, FileCabinetRecord> recordIdDictionary)
        {
            this.records = records.ToList();
            this.firstNameDictionary = firstNameDictionary;
            this.lastNameDictionary = lastNameDictionary;
            this.dateOfBirthDictionary = dateOfBirthDictionary;
            this.recordIdDictionary = recordIdDictionary;
            this.validator = validator;
        }

        /// <summary>
        /// Gets a value of the date format.
        /// </summary>
        /// <value>dateFormat.</value>
        public string DateFormat { get => this.validator.DateFormat; }

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
                return (this.records.Count, 0);
            }
        }

        /// <summary>
        /// Gets array of valid permissions.
        /// </summary>
        /// <returns>An array of valid permissions.</returns>
        public ReadOnlyCollection<char> GetValidPermissions() => this.validator.GetValidPermissions();

        /// <summary>
        /// Creates a new record and adds it into the records list.
        /// </summary>
        /// <param name="recordParameters">Parameters of the record to change.</param>
        /// <exception cref="ArgumentNullException">Thrown when recordParameters are null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when firstName or lastName is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firstName or lastName less than 2 or more than 60.</exception>
        /// <exception cref="ArgumentException">Thrown when firstName or lastName empty or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown when dateOfBirth less than "01-01-1950" or more than current date.</exception>
        /// <exception cref="ArgumentException">Thrown when salary less than 0.</exception>
        /// <returns>Id of the created record.</returns>
        public int CreateRecord(FileCabinetRecordParameterObject recordParameters)
        {
            ArgumentNullException.ThrowIfNull(recordParameters);

            this.validator.ValidateParameters(recordParameters);

            int newId = 1;

            while (this.GetRecords().Select(r => r.Id).Contains(newId))
            {
                newId++;
            }

            var record = new FileCabinetRecord
            {
                Id = newId,
                FirstName = recordParameters.FirstName,
                LastName = recordParameters.LastName,
                DateOfBirth = recordParameters.DateOfBirth,
                Status = recordParameters.Status,
                Salary = recordParameters.Salary,
                Permissions = recordParameters.Permissions,
            };

            this.recordIdDictionary.Add(record.Id, record);

            AddToDictionary(this.firstNameDictionary, recordParameters.FirstName.ToUpperInvariant(), record);
            AddToDictionary(this.lastNameDictionary, recordParameters.LastName.ToUpperInvariant(), record);
            AddToDictionary(this.dateOfBirthDictionary, recordParameters.DateOfBirth, record);
            this.records.Add(record);

            return record.Id;
        }

        /// <summary>
        /// Gets copy of the records as record array.
        /// </summary>
        /// <returns>Array of the records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            return this.records.AsReadOnly();
        }

        /// <summary>
        /// Edits the existing record with the specified id.
        /// </summary>
        /// <param name="id">Id of the record to edit.</param>
        /// <param name="recordParameters">Parameters of the record to change.</param>
        /// <exception cref="ArgumentNullException">Thrown when recordParameters are null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when firstName or lastName is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firstName or lastName less than 2 or more than 60.</exception>
        /// <exception cref="ArgumentException">Thrown when firstName or lastName empty or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown when dateOfBirth less than "01-01-1950" or more than current date.</exception>
        /// <exception cref="ArgumentException">Thrown when salary less than 0.</exception>
        /// <exception cref="ArgumentException">Thrown when record with the specified id isn't found.</exception>
        public void EditRecord(int id, FileCabinetRecordParameterObject recordParameters)
        {
            this.validator.ValidateParameters(recordParameters);
            bool isExistent = this.recordIdDictionary.ContainsKey(id);

            if (isExistent && recordParameters is not null)
            {
                FileCabinetRecord record = this.recordIdDictionary[id];
                this.firstNameDictionary[record.FirstName.ToUpperInvariant()].Remove(record);
                this.lastNameDictionary[record.LastName.ToUpperInvariant()].Remove(record);
                this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);

                record.FirstName = recordParameters.FirstName;
                record.LastName = recordParameters.LastName;
                record.DateOfBirth = recordParameters.DateOfBirth;
                record.Status = recordParameters.Status;
                record.Salary = recordParameters.Salary;
                record.Permissions = recordParameters.Permissions;

                AddToDictionary(this.firstNameDictionary, recordParameters.FirstName.ToUpperInvariant(), record);
                AddToDictionary(this.lastNameDictionary, recordParameters.LastName.ToUpperInvariant(), record);
                AddToDictionary(this.dateOfBirthDictionary, recordParameters.DateOfBirth, record);
            }
            else
            {
                throw new ArgumentException("Record with the specified id doesn't exist.", nameof(id));
            }
        }

        /// <summary>
        /// Looks for records with firstName property equal to the specified firstName parameter.
        /// </summary>
        /// <param name="firstName">First name of the records to seek.</param>
        /// <returns>Array of the found records with the specified firstName.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> foundRecords = new ();

            if (firstName is not null && this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
            {
                foundRecords = this.firstNameDictionary[firstName.ToUpperInvariant()];
            }

            return foundRecords.AsReadOnly();
        }

        /// <summary>
        /// Looks for records with lastName property equal to the specified lastName parameter.
        /// </summary>
        /// <param name="lastName">Last name of the records to seek.</param>
        /// <returns>Array of the found records with the specified lastName.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> foundRecords = new ();

            if (lastName is not null && this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
            {
                foundRecords = this.lastNameDictionary[lastName.ToUpperInvariant()];
            }

            return foundRecords.AsReadOnly();
        }

        /// <summary>
        /// Looks for records with dateOfBirth property equal to the specified date parameter.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth of the records to seek.</param>
        /// <returns>Array of the found records with the specified dateOfBirth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            List<FileCabinetRecord> foundRecords = new ();

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth))
            {
                foundRecords = this.dateOfBirthDictionary[dateOfBirth];
            }

            return foundRecords.AsReadOnly();
        }

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetDefaultService.
        /// </summary>
        public void ChangeValidatorToCustom()
        {
            this.validator = new CustomValidator();
        }

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetCustomService.
        /// </summary>
        public void ChangeValidatorToDefault()
        {
            this.validator = new DefaultValidator();
        }

        /// <summary>
        /// Makes snapshot of the FileCabinetMemoryService with the copy of the records.
        /// </summary>
        /// <returns>FileCabinetServiceSnapshot of the current FileCabinetMemoryService instance.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            return new FileCabinetServiceSnapshot(this.records);
        }

        /// <summary>
        /// Compares data from snapshot and updates records.
        /// </summary>
        /// <param name="snapshot">Snapshot to compare with.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            if (snapshot is not null)
            {
                List<FileCabinetRecord> newRecords = snapshot.Records.ToList();

                foreach (var rec in newRecords)
                {
                    Func<object, Tuple<bool, string>>[] validationMethods = new Func<object, Tuple<bool, string>>[] { p => this.validator.ValidateName(p as string), p => this.validator.ValidateName(p as string), p => this.validator.ValidateDateOfBirth(p as DateTime?), p => this.validator.ValidateStatus(p as short?), p => this.validator.ValidateSalary(p as decimal?), p => this.validator.ValidatePermissions(p as char?) };
                    object[] parameters = new object[] { rec.FirstName, rec.LastName, rec.DateOfBirth, rec.Status, rec.Salary, rec.Permissions };

                    for (int i = 0; i < validationMethods.Length; i++)
                    {
                        Tuple<bool, string> validationResult = validationMethods[i](parameters[i]);

                        if (!validationResult.Item1)
                        {
                            Console.WriteLine($"Record #{rec.Id}, {validationResult.Item2}, skips.");
                            continue;
                        }
                    }

                    this.records.RemoveAll(r => r.Id == rec.Id);
                    this.records.Add(rec);
                    this.recordIdDictionary[rec.Id] = rec;

                    if (this.firstNameDictionary.ContainsKey(rec.FirstName.ToUpperInvariant()))
                    {
                        for (int i = 0; i < this.firstNameDictionary[rec.FirstName.ToUpperInvariant()].Count; i++)
                        {
                            FileCabinetRecord fnameRec = this.firstNameDictionary[rec.FirstName.ToUpperInvariant()][i];

                            if (fnameRec.Id == rec.Id)
                            {
                                this.firstNameDictionary[rec.FirstName.ToUpperInvariant()].Remove(fnameRec);
                            }

                            this.firstNameDictionary[rec.FirstName.ToUpperInvariant()].Add(rec);
                        }
                    }
                    else
                    {
                        this.firstNameDictionary.Add(rec.FirstName.ToUpperInvariant(), new () { rec });
                    }

                    if (this.lastNameDictionary.ContainsKey(rec.LastName.ToUpperInvariant()))
                    {
                        for (int i = 0; i < this.lastNameDictionary[rec.LastName.ToUpperInvariant()].Count; i++)
                        {
                            FileCabinetRecord lnameRec = this.lastNameDictionary[rec.LastName.ToUpperInvariant()][i];

                            if (lnameRec.Id == rec.Id)
                            {
                                this.lastNameDictionary[rec.LastName.ToUpperInvariant()].Remove(lnameRec);
                            }

                            this.lastNameDictionary[rec.LastName.ToUpperInvariant()].Add(rec);
                        }
                    }
                    else
                    {
                        this.lastNameDictionary.Add(rec.LastName.ToUpperInvariant(), new () { rec });
                    }

                    if (this.dateOfBirthDictionary.ContainsKey(rec.DateOfBirth))
                    {
                        for (int i = 0; i < this.dateOfBirthDictionary[rec.DateOfBirth].Count; i++)
                        {
                            FileCabinetRecord dateRec = this.dateOfBirthDictionary[rec.DateOfBirth][i];

                            if (dateRec.Id == rec.Id)
                            {
                                this.dateOfBirthDictionary[rec.DateOfBirth].Remove(dateRec);
                            }

                            this.dateOfBirthDictionary[rec.DateOfBirth].Add(rec);
                        }
                    }
                    else
                    {
                        this.dateOfBirthDictionary.Add(rec.DateOfBirth, new () { rec });
                    }
                }
            }
        }

        private static void AddToDictionary<T>(Dictionary<T, List<FileCabinetRecord>> targetDictionary, T key, FileCabinetRecord record)
            where T : notnull
        {
            if (targetDictionary.ContainsKey(key))
            {
                targetDictionary[key].Add(record);
            }
            else
            {
                targetDictionary.Add(key, new List<FileCabinetRecord>() { record });
            }
        }

        public void RemoveRecord(int id)
        {
            for (int j = 0; j < this.records.Count; j++)
            {
                var record = this.records[j];

                if (record.Id == id)
                {
                    this.records.Remove(record);
                    this.recordIdDictionary.Remove(record.Id);

                    foreach (var dictElement in this.firstNameDictionary)
                    {
                        for (int i = 0; i < dictElement.Value.Count; i++)
                        {
                            var dictRecord = dictElement.Value[i];

                            if (dictRecord.Id == id)
                            {
                                dictElement.Value.Remove(dictRecord);
                            }
                        }
                    }

                    foreach (var dictElement in this.lastNameDictionary)
                    {
                        for (int i = 0; i < dictElement.Value.Count; i++)
                        {
                            var dictRecord = dictElement.Value[i];

                            if (dictRecord.Id == id)
                            {
                                dictElement.Value.Remove(dictRecord);
                            }
                        }
                    }

                    foreach (var dictElement in this.dateOfBirthDictionary)
                    {
                        for (int i = 0; i < dictElement.Value.Count; i++)
                        {
                            var dictRecord = dictElement.Value[i];

                            if (dictRecord.Id == id)
                            {
                                dictElement.Value.Remove(dictRecord);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes deleted records from source database.
        /// </summary>
        public void Purge()
        {
        }

        public Func<string?, Tuple<bool, string>> ValidateName()
        {
            return this.validator.ValidateName;
        }

        public Func<DateTime?, Tuple<bool, string>> ValidateDateOfBirth()
        {
            return this.validator.ValidateDateOfBirth;
        }

        public Func<short?, Tuple<bool, string>> ValidateStatus()
        {
            return this.validator.ValidateStatus;
        }

        public Func<decimal?, Tuple<bool, string>> ValidateSalary()
        {
            return this.validator.ValidateSalary;
        }

        public Func<char?, Tuple<bool, string>> ValidatePermissions()
        {
            return this.validator.ValidatePermissions;
        }
    }
}
