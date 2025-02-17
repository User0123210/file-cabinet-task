using FileCabinetApp.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Immutable;

#pragma warning disable SA1011

namespace FileCabinetApp
{
    /// <summary>
    /// Represents decorator for IFileCabinetService to enable execution time measurements.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private readonly IFileCabinetService service;

        /// <summary>
        /// Initializes new instance of the ServiceMeter class.
        /// </summary>
        /// <param name="service">Service to initialize service field.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Gets a value of the date format.
        /// </summary>
        /// <value>dateFormat.</value>
        public string DateFormat { get => this.service.DateFormat; }

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
                Stopwatch measure = new ();
                measure.Start();
                var stat = this.service.GetStat;
                measure.Stop();
                Console.WriteLine($"Get statistics method execution duration is {measure.ElapsedTicks} ticks.");
                return stat;
            }
        }

        /// <summary>
        /// Gets cached search data
        /// </summary>
        /// <value>Copy of the search cache dictionary.</value>
        public Dictionary<ImmutableArray<(string, string)>, IReadOnlyCollection<FileCabinetRecord>> SearchCache
        {
            get
            {
                Stopwatch measure = new ();
                measure.Start();
                var stat = this.service.SearchCache;
                measure.Stop();
                Console.WriteLine($"Get search cache execution duration is {measure.ElapsedTicks} ticks.");
                return stat;
            }
        }

        /// <summary>
        /// Gets array of validators to validate records for the service.
        /// </summary>
        /// <returns>Array of validators.</returns>
        public IRecordValidator[]? GetValidators()
        {
            return this.service.GetValidators();
        }

        /// <summary>
        /// Creates a new record and adds it into the records list.
        /// </summary>
        /// <param name="recordParameters">Parameters of the record to add.</param>
        /// <returns>Id of the created record.</returns>
        public int CreateRecord(FileCabinetRecordParameterObject recordParameters)
        {
            Stopwatch measure = new ();
            measure.Start();
            int id = this.service.CreateRecord(recordParameters);
            measure.Stop();
            Console.WriteLine($"Create method execution duration is {measure.ElapsedTicks} ticks.");
            return id;
        }

        /// <summary>
        /// Creates a new record and adds it into the records list.
        /// </summary>
        /// <param name="id">Id of the record to add.</param>
        /// <param name="recordParameters">Parameters of the record to add.</param>
        /// <returns>Id of the created record.</returns>
        public int CreateRecord(int id, FileCabinetRecordParameterObject recordParameters)
        {
            Stopwatch measure = new ();
            measure.Start();
            this.service.CreateRecord(id, recordParameters);
            measure.Stop();
            Console.WriteLine($"Create method execution duration is {measure.ElapsedTicks} ticks.");
            return id;
        }

        /// <summary>
        /// Gets copy of the records as record array.
        /// </summary>
        /// <returns>Array of the records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            Stopwatch measure = new ();
            measure.Start();
            IEnumerable<FileCabinetRecord> records = this.service.GetRecords();
            measure.Stop();
            Console.WriteLine($"Get records method execution duration is {measure.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// Adds cache data to the cache.
        /// </summary>
        public void AddToSearchCache(ImmutableArray<(string, string)> criteria, IReadOnlyCollection<FileCabinetRecord> data)
        {
            Stopwatch measure = new ();
            measure.Start();
            this.service.AddToSearchCache(criteria, data);
            measure.Stop();
            Console.WriteLine($"Add to search cache method execution duration is {measure.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Edits the existing record with the specified id.
        /// </summary>
        /// <param name="id">Id of the record to edit.</param>
        /// <param name="recordParameters">Parameters of the record to change.</param>
        public void EditRecord(int id, FileCabinetRecordParameterObject recordParameters)
        {
            Stopwatch measure = new ();
            measure.Start();
            this.service.EditRecord(id, recordParameters);
            measure.Stop();
            Console.WriteLine($"Edit method execution duration is {measure.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Looks for records with firstName property equal to the specified firstName parameter.
        /// </summary>
        /// <param name="firstName">First name of the records to seek.</param>
        /// <returns>Array of the found records with the specified firstName.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            Stopwatch measure = new ();
            measure.Start();
            IEnumerable<FileCabinetRecord> records = this.service.FindByFirstName(firstName);
            measure.Stop();
            Console.WriteLine($"Find by first name method execution duration is {measure.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// Looks for records with lastName property equal to the specified lastName parameter.
        /// </summary>
        /// <param name="lastName">Last name of the records to seek.</param>
        /// <returns>Array of the found records with the specified lastName.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            Stopwatch measure = new ();
            measure.Start();
            IEnumerable<FileCabinetRecord> records = this.service.FindByLastName(lastName);
            measure.Stop();
            Console.WriteLine($"Find by last name method execution duration is {measure.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// Looks for records with dateOfBirth property equal to the specified date parameter.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth of the records to seek.</param>
        /// <returns>Array of the found records with the specified dateOfBirth.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            Stopwatch measure = new ();
            measure.Start();
            IEnumerable<FileCabinetRecord> records = this.service.FindByDateOfBirth(dateOfBirth);
            measure.Stop();
            Console.WriteLine($"Find by date of birth method execution duration is {measure.ElapsedTicks} ticks.");
            return records;
        }

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetDefaultService.
        /// </summary>
        public void ChangeValidatorToCustom()
        {
            Stopwatch measure = new ();
            measure.Start();
            this.service.ChangeValidatorToCustom();
            measure.Stop();
            Console.WriteLine($"Change validator to custom method execution duration is {measure.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetCustomService.
        /// </summary>
        public void ChangeValidatorToDefault()
        {
            Stopwatch measure = new ();
            measure.Start();
            this.service.ChangeValidatorToDefault();
            measure.Stop();
            Console.WriteLine($"Change validator to default method execution duration is {measure.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Makes snapshot of the IFileCabinetService with the copy of the records.
        /// </summary>
        /// <returns>FileCabinetServiceSnapshot of the current IFileCabinetService instance.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            Stopwatch measure = new ();
            measure.Start();
            FileCabinetServiceSnapshot snapshot = this.service.MakeSnapshot();
            measure.Stop();
            Console.WriteLine($"Make snapshot method execution duration is {measure.ElapsedTicks} ticks.");
            return snapshot;
        }

        /// <summary>
        /// Compares data from snapshot and updates records.
        /// </summary>
        /// <param name="snapshot">Snapshot to compare with.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            Stopwatch measure = new ();
            measure.Start();
            this.service.Restore(snapshot);
            measure.Stop();
            Console.WriteLine($"Restore method execution duration is {measure.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Removes record with the specified id.
        /// </summary>
        /// <param name="id">Id of the record to delete.</param>
        public void RemoveRecord(int id)
        {
            Stopwatch measure = new ();
            measure.Start();
            this.service.RemoveRecord(id);
            measure.Stop();
            Console.WriteLine($"Remove method execution duration is {measure.ElapsedTicks} ticks.");
        }

        /// <summary>
        /// Removes deleted records from source database.
        /// </summary>
        public void Purge()
        {
            Stopwatch measure = new ();
            measure.Start();
            this.service.Purge();
            measure.Stop();
            Console.WriteLine($"Purge method execution duration is {measure.ElapsedTicks} ticks.");
        }
    }
}
