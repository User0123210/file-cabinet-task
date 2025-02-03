using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;

#pragma warning disable CA1024

namespace FileCabinetApp
{
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();
        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        public FileCabinetService()
        {
            this.list = new List<FileCabinetRecord>();
        }

        public FileCabinetService(Collection<FileCabinetRecord> list)
        {
            this.list = list.ToList<FileCabinetRecord>();
        }

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short status, decimal salary, char permissions)
        {
            ValidateParameters(firstName, lastName, dateOfBirth, status, salary, permissions);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Status = status,
                Salary = salary,
                Permissions = permissions,
            };

            if (this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
            {
                this.firstNameDictionary[firstName.ToUpperInvariant()].Add(record);
            }
            else
            {
                this.firstNameDictionary.Add(firstName.ToUpperInvariant(), new List<FileCabinetRecord>() { record });
            }

            this.list.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            return this.list.ToArray();
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short status, decimal salary, char permissions)
        {
            ValidateParameters(firstName, lastName, dateOfBirth, status, salary, permissions);
            bool isExistent = false;

            foreach (var record in this.list)
            {
                if (record.Id == id)
                {
                    this.firstNameDictionary[record.FirstName.ToUpperInvariant()].Remove(record);

                    record.FirstName = firstName;
                    record.LastName = lastName;
                    record.DateOfBirth = dateOfBirth;
                    record.Status = status;
                    record.Salary = salary;
                    record.Permissions = permissions;
                    isExistent = true;

                    if (this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
                    {
                        this.firstNameDictionary[firstName.ToUpperInvariant()].Add(record);
                    }
                    else
                    {
                        this.firstNameDictionary.Add(firstName.ToUpperInvariant(), new List<FileCabinetRecord>() { record });
                    }
                }
            }

            if (!isExistent)
            {
                throw new ArgumentException("Record with the specified id doesn't exist.");
            }
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();

            if (firstName is not null && this.firstNameDictionary.ContainsKey(firstName.ToUpperInvariant()))
            {
                foundRecords = this.firstNameDictionary[firstName.ToUpperInvariant()];
            }

            return foundRecords.ToArray();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            List<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();

            foreach (FileCabinetRecord record in this.list)
            {
                if (string.Equals(record.LastName, lastName, StringComparison.OrdinalIgnoreCase))
                {
                    foundRecords.Add(record);
                }
            }

            return foundRecords.ToArray();
        }

        public FileCabinetRecord[] FindByDateOfBirth(DateTime date)
        {
            List<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();

            foreach (FileCabinetRecord record in this.list)
            {
                if (DateTime.Equals(record.DateOfBirth, date))
                {
                    foundRecords.Add(record);
                }
            }

            return foundRecords.ToArray();
        }

        private static (string firstName, string lastName, DateTime dateOfBirth, short status, decimal salary, char permissions) ValidateParameters(string firstName, string lastName, DateTime dateOfBirth, short status, decimal salary, char permissions)
        {
            ArgumentNullException.ThrowIfNull(firstName);

            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("First name shouldn't be empty or whitespace", nameof(firstName));
            }

            if (firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException("First name's length should be more or equal 2 and less or equal 60", nameof(firstName));
            }

            ArgumentNullException.ThrowIfNull(lastName);

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Last name shouldn't be empty or whitespace", nameof(lastName));
            }

            if (lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException("Last name's length should be more or equal 2 and less or equal 60", nameof(lastName));
            }

            if (dateOfBirth < DateTime.ParseExact("01-01-1950", "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None) || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth shouldn't be less than 01-Jan-1950 or more than current date.", nameof(dateOfBirth));
            }

            if (salary < 0)
            {
                throw new ArgumentException("Salary value shouldn't be less than 0", nameof(salary));
            }

            return (firstName, lastName, dateOfBirth, status, salary, permissions);
        }
    }
}
