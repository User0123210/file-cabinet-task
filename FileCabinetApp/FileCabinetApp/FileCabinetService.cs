using System;
using System.Collections;
using System.Collections.ObjectModel;
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
