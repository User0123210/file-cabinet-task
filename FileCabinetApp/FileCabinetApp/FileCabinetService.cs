using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;

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

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth)
        {
            // TODO: добавьте реализацию метода
            return 0;
        }

        public FileCabinetRecord[] GetRecords()
        {
            // TODO: добавьте реализацию метода
            return Array.Empty<FileCabinetRecord>();
        }

        public int GetStat()
        {
            return this.list.Count;
        }
    }
}
