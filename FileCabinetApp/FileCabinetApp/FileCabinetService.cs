using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Manages information about the records in file cabinet.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> records;
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();
        private readonly Dictionary<int, FileCabinetRecord> recordIdDictionary = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// Initializes records with empty list of records.
        /// </summary>
        public FileCabinetService()
        {
            this.records = new List<FileCabinetRecord>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// Initializes records with the specified list of records.
        /// </summary>
        /// <param name="records">list of existing records to initialize records field.</param>
        public FileCabinetService(Collection<FileCabinetRecord> records)
        {
            this.records = records.ToList();
        }

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
            ValidateParameters(recordParameters);

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
        public FileCabinetRecord[] GetRecords()
        {
            return this.records.ToArray();
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
            ValidateParameters(recordParameters);
            bool isExistent = this.recordIdDictionary.ContainsKey(id);

            if (isExistent)
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
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> foundRecords = new ();

            if (firstName is not null && this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
            {
                foundRecords = this.firstNameDictionary[firstName.ToUpperInvariant()];
            }

            return foundRecords.ToArray();
        }

        /// <summary>
        /// Looks for records with lastName property equal to the specified lastName parameter.
        /// </summary>
        /// <param name="lastName">Last name of the records to seek.</param>
        /// <returns>Array of the found records with the specified lastName.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            List<FileCabinetRecord> foundRecords = new ();

            if (lastName is not null && this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
            {
                foundRecords = this.lastNameDictionary[lastName.ToUpperInvariant()];
            }

            return foundRecords.ToArray();
        }

        /// <summary>
        /// Looks for records with dateOfBirth property equal to the specified date parameter.
        /// </summary>
        /// <param name="date">Date of birth of the records to seek.</param>
        /// <returns>Array of the found records with the specified dateOfBirth.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(DateTime date)
        {
            List<FileCabinetRecord> foundRecords = new ();

            if (this.dateOfBirthDictionary.ContainsKey(date))
            {
                foundRecords = this.dateOfBirthDictionary[date];
            }

            return foundRecords.ToArray();
        }

        private static void ValidateParameters(FileCabinetRecordParameterObject? recordParameters)
        {
            int minLength = 2;
            int maxLength = 60;

            ArgumentNullException.ThrowIfNull(recordParameters);
            ArgumentNullException.ThrowIfNull(recordParameters.FirstName);

            if (string.IsNullOrWhiteSpace(recordParameters.FirstName))
            {
                throw new ArgumentException("First name shouldn't be empty or whitespace", nameof(recordParameters));
            }

            if (recordParameters.FirstName.Length < minLength || recordParameters.FirstName.Length > maxLength)
            {
                throw new ArgumentException($"First name's length should be more or equal {minLength} and less or equal {maxLength}", nameof(recordParameters));
            }

            ArgumentNullException.ThrowIfNull(recordParameters.LastName);

            if (string.IsNullOrWhiteSpace(recordParameters.LastName))
            {
                throw new ArgumentException("Last name shouldn't be empty or whitespace", nameof(recordParameters));
            }

            if (recordParameters.LastName.Length < minLength || recordParameters.LastName.Length > maxLength)
            {
                throw new ArgumentException($"Last name's length should be more or equal {minLength} and less or equal {maxLength}", nameof(recordParameters));
            }

            DateTime minDate = DateTime.ParseExact("01-01-1950", "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

            if (recordParameters.DateOfBirth < minDate || recordParameters.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException($"Date of birth shouldn't be less than {minDate} or more than current date.", nameof(recordParameters));
            }

            if (recordParameters.Salary < 0)
            {
                throw new ArgumentException("Salary value shouldn't be less than 0", nameof(recordParameters));
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
