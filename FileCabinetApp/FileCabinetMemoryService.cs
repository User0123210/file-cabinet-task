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
        /// Gets minimal possible length of the name.
        /// </summary>
        /// <value>this.minNameLength.</value>
        public int MinNameLength { get => this.validator.MinNameLength; }

        /// <summary>
        /// Gets maximum possible length of the name.
        /// </summary>
        /// <value>this.maxNameLength.</value>
        public int MaxNameLength { get => this.validator.MaxNameLength; }

        /// <summary>
        /// Gets a value indicating whether the name should contain only letter characters or not.
        /// </summary>
        /// <value>isOnlyLetterName.</value>
        public bool IsOnlyLetterName { get => this.validator.IsOnlyLetterName; }

        /// <summary>
        /// Gets a value of the date format.
        /// </summary>
        /// <value>dateFormat.</value>
        public string DateFormat { get => this.validator.DateFormat; }

        /// <summary>
        /// Gets minimum possible date.
        /// </summary>
        /// <value>this.minDate.</value>
        public DateTime MinDate { get => this.validator.MinDate; }

        /// <summary>
        /// Gets information about the number of records in the service.
        /// </summary>
        /// <value>
        /// <records.Count>Information about the number of records in the service.</records.Count>
        /// </value>
        public int GetStat
        {
            get
            {
                return this.records.Count;
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
            this.validator.ValidateParameters(recordParameters);

            ArgumentNullException.ThrowIfNull(recordParameters);

            var record = new FileCabinetRecord
            {
                Id = this.records.Count + 1,
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
        /// <param name="date">Date of birth of the records to seek.</param>
        /// <returns>Array of the found records with the specified dateOfBirth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime date)
        {
            List<FileCabinetRecord> foundRecords = new ();

            if (this.dateOfBirthDictionary.ContainsKey(date))
            {
                foundRecords = this.dateOfBirthDictionary[date];
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
                    if (string.IsNullOrWhiteSpace(rec.FirstName))
                    {
                        Console.WriteLine($"Record #{rec.Id}, empty or whitespace firstName, skips.");
                        continue;
                    }

                    if (rec.FirstName.Length > this.validator.MaxNameLength || rec.FirstName.Length < this.validator.MinNameLength)
                    {
                        Console.WriteLine($"Record #{rec.Id}, firstName length outside of {this.validator.MinNameLength}-{this.validator.MaxNameLength} range, skips.");
                        continue;
                    }

                    if (this.validator.IsOnlyLetterName)
                    {
                        foreach (var letter in rec.FirstName)
                        {
                            if (!char.IsLetter(letter))
                            {
                                Console.WriteLine($"Record #{rec.Id}, firstName contains non letter characters, skips.");
                                continue;
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(rec.LastName))
                    {
                        Console.WriteLine($"Record #{rec.Id}, empty or whitespace lastName, skips.");
                        continue;
                    }

                    if (rec.LastName.Length > this.validator.MaxNameLength || rec.LastName.Length < this.validator.MinNameLength)
                    {
                        Console.WriteLine($"Record #{rec.Id}, lastName length outside of {this.validator.MinNameLength}-{this.validator.MaxNameLength} range, skips.");
                        continue;
                    }

                    if (this.validator.IsOnlyLetterName)
                    {
                        foreach (var letter in rec.LastName)
                        {
                            if (!char.IsLetter(letter))
                            {
                                Console.WriteLine($"Record #{rec.Id}, lastName contains non letter characters, skips.");
                                continue;
                            }
                        }
                    }

                    if (rec.DateOfBirth < this.validator.MinDate || rec.DateOfBirth > DateTime.Now)
                    {
                        Console.WriteLine($"Record #{rec.Id}, dateOfBirth outside of range: ({this.validator.MinDate}) - ({DateTime.Now}), skips.");
                        continue;
                    }

                    if (rec.Salary < 0)
                    {
                        Console.WriteLine($"Record #{rec.Id}, salary is less than zero, skips.");
                        continue;
                    }

                    if (this.validator.GetValidPermissions().Count > 0)
                    {
                        bool isValid = false;

                        foreach (char c in this.validator.GetValidPermissions())
                        {
                            if (c == rec.Permissions)
                            {
                                isValid = true;
                            }
                        }

                        if (!isValid)
                        {
                            Console.WriteLine($"Record #{rec.Id}, permissions is not one of valid permissions, skips.");
                            continue;
                        }
                    }

                    this.records.RemoveAll(r => r.Id == rec.Id);
                    this.records.Add(rec);
                    this.recordIdDictionary[rec.Id] = rec;

                    if (this.firstNameDictionary.ContainsKey(rec.FirstName))
                    {
                        for (int i = 0; i < this.firstNameDictionary[rec.FirstName].Count; i++)
                        {
                            FileCabinetRecord fnameRec = this.firstNameDictionary[rec.FirstName][i];

                            if (fnameRec.Id == rec.Id)
                            {
                                this.firstNameDictionary[rec.FirstName].Remove(fnameRec);
                            }

                            this.firstNameDictionary[rec.FirstName].Add(rec);
                        }
                    }
                    else
                    {
                        this.firstNameDictionary.Add(rec.FirstName, new () { rec });
                    }

                    if (this.lastNameDictionary.ContainsKey(rec.LastName))
                    {
                        for (int i = 0; i < this.lastNameDictionary[rec.LastName].Count; i++)
                        {
                            FileCabinetRecord lnameRec = this.lastNameDictionary[rec.LastName][i];

                            if (lnameRec.Id == rec.Id)
                            {
                                this.lastNameDictionary[rec.LastName].Remove(lnameRec);
                            }

                            this.lastNameDictionary[rec.LastName].Add(rec);
                        }
                    }
                    else
                    {
                        this.lastNameDictionary.Add(rec.LastName, new () { rec });
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
    }
}
