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

            if (dateOfBirth < DateTime.ParseExact("01-01-1950", "dd-MM-yyyy", CultureInfo.InvariantCulture,  DateTimeStyles.None) || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date of birth shouldn't be less than 01-Jan-1950 or more than current date.", nameof(dateOfBirth));
            }

            if (salary < 0)
            {
                throw new ArgumentException("Salary value shouldn't be less than 0", nameof(salary));
            }

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
    }
}
