using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> records = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new ();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new ();
        private readonly Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary = new ();
        private readonly Dictionary<int, FileCabinetRecord> recordIdDictionary = new ();

        public FileCabinetService()
        {
            this.records = new List<FileCabinetRecord>();
        }

        public FileCabinetService(Collection<FileCabinetRecord> records)
        {
            this.records = records.ToList();
        }

        public int GetStat
        {
            get
            {
                return this.records.Count;
            }
        }

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short status, decimal salary, char permissions)
        {
            ValidateParameters(firstName, lastName, dateOfBirth, status, salary, permissions);

            var record = new FileCabinetRecord
            {
                Id = this.records.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Status = status,
                Salary = salary,
                Permissions = permissions,
            };

            this.recordIdDictionary.Add(record.Id, record);

            AddToDictionary(this.firstNameDictionary, firstName.ToUpperInvariant(), record);
            AddToDictionary(this.lastNameDictionary, lastName.ToUpperInvariant(), record);
            AddToDictionary(this.dateOfBirthDictionary, dateOfBirth, record);
            this.records.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.records.ToArray();
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short status, decimal salary, char permissions)
        {
            ValidateParameters(firstName, lastName, dateOfBirth, status, salary, permissions);
            bool isExistent = this.recordIdDictionary.ContainsKey(id);

            if (isExistent)
            {
                FileCabinetRecord record = this.recordIdDictionary[id];
                this.firstNameDictionary[record.FirstName.ToUpperInvariant()].Remove(record);
                this.lastNameDictionary[record.LastName.ToUpperInvariant()].Remove(record);
                this.dateOfBirthDictionary[record.DateOfBirth].Remove(record);

                record.FirstName = firstName;
                record.LastName = lastName;
                record.DateOfBirth = dateOfBirth;
                record.Status = status;
                record.Salary = salary;
                record.Permissions = permissions;

                AddToDictionary(this.firstNameDictionary, firstName.ToUpperInvariant(), record);
                AddToDictionary(this.lastNameDictionary, lastName.ToUpperInvariant(), record);
                AddToDictionary(this.dateOfBirthDictionary, dateOfBirth, record);
            }
            else
            {
                throw new ArgumentException("Record with the specified id doesn't exist.");
            }
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> foundRecords = new ();

            if (firstName is not null && this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
            {
                foundRecords = this.firstNameDictionary[firstName.ToUpperInvariant()];
            }

            return foundRecords.ToArray();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            List<FileCabinetRecord> foundRecords = new ();

            if (lastName is not null && this.lastNameDictionary.ContainsKey(lastName.ToUpperInvariant()))
            {
                foundRecords = this.lastNameDictionary[lastName.ToUpperInvariant()];
            }

            return foundRecords.ToArray();
        }

        public FileCabinetRecord[] FindByDateOfBirth(DateTime date)
        {
            List<FileCabinetRecord> foundRecords = new ();

            if (this.dateOfBirthDictionary.ContainsKey(date))
            {
                foundRecords = this.dateOfBirthDictionary[date];
            }

            return foundRecords.ToArray();
        }

        private static (string firstName, string lastName, DateTime dateOfBirth, short status, decimal salary, char permissions) ValidateParameters(string firstName, string lastName, DateTime dateOfBirth, short status, decimal salary, char permissions)
        {
            int minLength = 2;
            int maxLength = 60;

            ArgumentNullException.ThrowIfNull(firstName);

            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("First name shouldn't be empty or whitespace", nameof(firstName));
            }

            if (firstName.Length < minLength || firstName.Length > maxLength)
            {
                throw new ArgumentException($"First name's length should be more or equal {minLength} and less or equal {maxLength}", nameof(firstName));
            }

            ArgumentNullException.ThrowIfNull(lastName);

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Last name shouldn't be empty or whitespace", nameof(lastName));
            }

            if (lastName.Length < minLength || lastName.Length > maxLength)
            {
                throw new ArgumentException($"Last name's length should be more or equal {minLength} and less or equal {maxLength}", nameof(lastName));
            }

            DateTime minDate = DateTime.ParseExact("01-01-1950", "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None);

            if (dateOfBirth < minDate || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException($"Date of birth shouldn't be less than {minDate} or more than current date.", nameof(dateOfBirth));
            }

            if (salary < 0)
            {
                throw new ArgumentException("Salary value shouldn't be less than 0", nameof(salary));
            }

            return (firstName, lastName, dateOfBirth, status, salary, permissions);
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
