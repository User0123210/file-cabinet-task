using FileCabinetApp.Validators;
using System;
using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

#pragma warning disable SA1011

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
        private readonly Dictionary<ImmutableArray<(string, string)>, IReadOnlyCollection<FileCabinetRecord>> searchCache = new (new CacheKeyComparer());

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
        /// Gets cached search data
        /// </summary>
        /// <value>Copy of the search cache dictionary.</value>
        public Dictionary<ImmutableArray<(string, string)>, IReadOnlyCollection<FileCabinetRecord>> SearchCache
        {
            get => this.searchCache;
        }

        /// Gets array of the validators to validate records in the service.        /// <summary>
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
                return (this.records.Count, 0);
            }
        }

        /// <summary>
        /// Creates a new record and adds it into the records list.
        /// </summary>
        /// <param name="recordParameters">Parameters of the record to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when recordParameters are null.</exception>
        /// <exception cref="ArgumentException">Thrown when firstName isn't valid.</exception>
        /// <exception cref="ArgumentException">Thrown when lastName isn't valid.</exception>
        /// <exception cref="ArgumentException">Thrown when dateOfBirth isn't valid.</exception>
        /// <exception cref="ArgumentException">Thrown when salary isn't valid.</exception>
        /// <returns>Id of the created record.</returns>
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
        /// Creates a new record and adds it into the records list.
        /// </summary>
        /// <param name="id">Id of the record to add.</param>
        /// <param name="recordParameters">Parameters of the record to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when recordParameters are null.</exception>
        /// <exception cref="ArgumentException">Thrown when firstName isn't valid.</exception>
        /// <exception cref="ArgumentException">Thrown when lastName isn't valid.</exception>
        /// <exception cref="ArgumentException">Thrown when dateOfBirth isn't valid.</exception>
        /// <exception cref="ArgumentException">Thrown when salary isn't valid.</exception>
        /// <exception cref="ArgumentException">Thrown when record with the specified id already exists.</exception>
        /// <returns>Id of the created record.</returns>
        public int CreateRecord(int id, FileCabinetRecordParameterObject recordParameters)
        {
            ArgumentNullException.ThrowIfNull(recordParameters);

            (bool isValid, string message) = this.validator.ValidateParameters(recordParameters);

            if (!isValid)
            {
                throw new ArgumentException(message);
            }

            if (this.recordIdDictionary.ContainsKey(id))
            {
                throw new ArgumentException("Record with the specified id already exists.", nameof(id));
            }

            var record = new FileCabinetRecord
            {
                Id = id,
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
            this.searchCache.Clear();

            return record.Id;
        }

        /// <summary>
        /// Gets copy of the records as record array.
        /// </summary>
        /// <returns>Array of the records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            return this.records.AsReadOnly();
        }

        /// <summary>
        /// Edits the existing record with the specified id.
        /// </summary>
        /// <param name="id">Id of the record to edit.</param>
        /// <param name="recordParameters">Parameters of the record to change.</param>
        /// <exception cref="ArgumentNullException">Thrown when recordParameters are null.</exception>
        /// <exception cref="ArgumentException">Thrown when firstName isn't valid.</exception>
        /// <exception cref="ArgumentException">Thrown when lastName isn't valid.</exception>
        /// <exception cref="ArgumentException">Thrown when dateOfBirth isn't valid.</exception>
        /// <exception cref="ArgumentException">Thrown when salary isn't valid.</exception>
        /// <exception cref="ArgumentException">Thrown when record with the specified id isn't found.</exception>
        public void EditRecord(int id, FileCabinetRecordParameterObject recordParameters)
        {
            (bool isValid, string message) = this.validator.ValidateParameters(recordParameters);

            if (!isValid)
            {
                throw new ArgumentException(message);
            }

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
                this.searchCache.Clear();
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
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
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
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
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
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
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
            this.validator = new ValidatorBuilder().CreateCustom();
        }

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetCustomService.
        /// </summary>
        public void ChangeValidatorToDefault()
        {
            this.validator = new ValidatorBuilder().CreateDefault();
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
        /// Adds cache data to the cache.
        /// </summary>
        public void AddToSearchCache(ImmutableArray<(string, string)> criteria, IReadOnlyCollection<FileCabinetRecord> data)
        {
            if (!this.searchCache.ContainsKey(criteria))
            {
                this.searchCache.Add(criteria, data);
            }
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
                    Tuple<bool, string> validationResult = this.validator.ValidateParameters(new FileCabinetRecordParameterObject() { FirstName = rec.FirstName, LastName = rec.LastName, DateOfBirth = rec.DateOfBirth, Status = rec.Status, Salary = rec.Salary, Permissions = rec.Permissions });

                    if (!validationResult.Item1)
                    {
                        Console.WriteLine($"Record #{rec.Id}, {validationResult.Item2}, skips.");
                        continue;
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

                this.searchCache.Clear();
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

        /// <summary>
        /// Removes record with the specified id from the list of records in the service.
        /// </summary>
        /// <param name="id">Id of the record to remove.</param>
        public void RemoveRecord(int id)
        {
            if (this.recordIdDictionary.ContainsKey(id))
            {
                FileCabinetRecord record = this.recordIdDictionary[id];

                this.records.Remove(record);

                this.recordIdDictionary.Remove(id);

                if (this.firstNameDictionary.ContainsKey(record.FirstName.ToUpperInvariant()))
                {
                     this.firstNameDictionary[record.FirstName.ToUpperInvariant()].Remove(record);
                }

                if (this.lastNameDictionary.ContainsKey(record.LastName.ToUpperInvariant()))
                {
                    this.lastNameDictionary[record.LastName.ToUpperInvariant()].Remove(record);
                }

                if (this.dateOfBirthDictionary.ContainsKey(record.DateOfBirth))
                {
                    this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);
                }

                this.searchCache.Clear();
            }
        }

        /// <summary>
        /// Removes deleted records from source database.
        /// </summary>
        public void Purge()
        {
        }
    }
}
