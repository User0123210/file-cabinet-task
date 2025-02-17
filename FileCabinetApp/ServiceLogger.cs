using FileCabinetApp.Validators;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Security;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable SA1011

namespace FileCabinetApp
{
    /// <summary>
    /// Represents decorator for IFileCabinetService to log executed operations.
    /// </summary>
    public class ServiceLogger : IFileCabinetService
    {
        private readonly IFileCabinetService service;
        private readonly string path = @"log.txt";

        /// <summary>
        /// Initializes new instance of the ServiceMeter class.
        /// </summary>
        /// <param name="service">Service to initialize service field.</param>
        public ServiceLogger(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Initializes new instance of the ServiceMeter class.
        /// </summary>
        /// <param name="service">Service to initialize service field.</param>
        /// <param name="logPath">Path where log should be written.</param>
        public ServiceLogger(IFileCabinetService service, string logPath)
        {
            FileStream fileStream;
            try
            {
                fileStream = new (logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                fileStream.Close();
                this.path = logPath;
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Destination directory not found: {ex.Message}.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Invalid destination directory: {ex.Message}.");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Can't get access to the destination directory: {ex.Message}.");
            }

            this.service = service;
        }

        /// <summary>
        /// Gets a value of the date format.
        /// </summary>
        /// <value>dateFormat.</value>
        public string DateFormat
        {
            get
            {
                FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                using StreamWriter writer = new (fileStream);
                writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Getting DateFormat, DateFormat get = {this.service.DateFormat}.");
                return this.service.DateFormat;
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
                FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                using StreamWriter writer = new (fileStream);
                writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Getting SearchCache.");
                return this.service.SearchCache;
            }
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
                FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                using StreamWriter writer = new (fileStream);

                writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Getting GetStat, GetStat get = {this.service.GetStat}.");
                var stat = this.service.GetStat;
                return stat;
            }
        }

        /// <summary>
        /// Gets array of validators to validate records for the service.
        /// </summary>
        /// <returns>Array of validators.</returns>
        public IRecordValidator[]? GetValidators()
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);

            var validators = this.service.GetValidators();
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling GetValidators() returned {validators?.Length} validators.");
            return validators;
        }

        /// <summary>
        /// Adds cache data to the cache.
        /// </summary>
        public void AddToSearchCache(ImmutableArray<(string, string)> criteria, IReadOnlyCollection<FileCabinetRecord> data)
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling AddToSearchCache() with key = {string.Join(", ", criteria.Select(c => c.Item1 + "=" + c.Item2))}.");
            this.service.AddToSearchCache(criteria, data);
        }

        /// <summary>
        /// Creates a new record and adds it into the records list.
        /// </summary>
        /// <param name="recordParameters">Parameters of the record to change.</param>
        /// <returns>Id of the created record.</returns>
        public int CreateRecord(FileCabinetRecordParameterObject recordParameters)
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);

            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling CreateRecord() with FirstName = {recordParameters?.FirstName}, LastName = {recordParameters?.LastName}, DateOfBirth = {recordParameters?.DateOfBirth}, Status = {recordParameters?.Status}, Salary = {recordParameters?.Salary}, Permissions = {recordParameters?.Permissions}.");

            try
            {
                int id = this.service.CreateRecord(recordParameters!);
                writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - CreateRecord() returned {id}.");
                return id;
            }
            catch (ArgumentNullException ex)
            {
                writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - CreateRecord() throw argumentNull exception with the message {ex.Message}.");
                throw;
            }
            catch (ArgumentException ex)
            {
                writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - CreateRecord() throw argument exception with the message {ex.Message}.");
                throw;
            }
        }

                /// <summary>
        /// Creates a new record and adds it into the records list.
        /// </summary>
        /// <param name="id">Id of the record to add.</param>
        /// <param name="recordParameters">Parameters of the record to add.</param>
        /// <returns>Id of the created record.</returns>
        public int CreateRecord(int id, FileCabinetRecordParameterObject recordParameters)
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);

            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling CreateRecord() with FirstName = {recordParameters?.FirstName}, LastName = {recordParameters?.LastName}, DateOfBirth = {recordParameters?.DateOfBirth}, Status = {recordParameters?.Status}, Salary = {recordParameters?.Salary}, Permissions = {recordParameters?.Permissions}.");

            try
            {
                this.service.CreateRecord(id, recordParameters!);
                writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - CreateRecord() returned {id}.");
                return id;
            }
            catch (ArgumentNullException ex)
            {
                writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - CreateRecord() throw argumentNull exception with the message {ex.Message}.");
                throw;
            }
            catch (ArgumentException ex)
            {
                writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - CreateRecord() throw argument exception with the message {ex.Message}.");
                throw;
            }
        }

        /// <summary>
        /// Gets copy of the records as record array.
        /// </summary>
        /// <returns>Array of the records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords()
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);

            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling GetRecords().");
            IEnumerable<FileCabinetRecord> records = this.service.GetRecords();
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - GetRecords() returned {records.Count()} records.");
            return records;
        }

        /// <summary>
        /// Edits the existing record with the specified id.
        /// </summary>
        /// <param name="id">Id of the record to edit.</param>
        /// <param name="recordParameters">Parameters of the record to change.</param>
        public void EditRecord(int id, FileCabinetRecordParameterObject recordParameters)
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);

            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling EditRecord() with id = {id}, FirstName = {recordParameters?.FirstName}, LastName = {recordParameters?.LastName}, DateOfBirth = {recordParameters?.DateOfBirth}, Status = {recordParameters?.Status}, Salary = {recordParameters?.Salary}, Permissions = {recordParameters?.Permissions}.");
            try
            {
                this.service.EditRecord(id, recordParameters!);
                writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - EditRecord() successfully completed.");
            }
            catch (ArgumentNullException ex)
            {
                writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - EditRecord() throw argumentNull exception with the message {ex.Message}.");
                throw;
            }
            catch (ArgumentException ex)
            {
                writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - EditRecord() throw argument exception with the message {ex.Message}.");
                throw;
            }
        }

        /// <summary>
        /// Looks for records with firstName property equal to the specified firstName parameter.
        /// </summary>
        /// <param name="firstName">First name of the records to seek.</param>
        /// <returns>Array of the found records with the specified firstName.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling FindByFirstName() with firstName = {firstName}.");
            IEnumerable<FileCabinetRecord> records = this.service.FindByFirstName(firstName);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - FindByFirstName() returned {records.Count()} records.");
            return records;
        }

        /// <summary>
        /// Looks for records with lastName property equal to the specified lastName parameter.
        /// </summary>
        /// <param name="lastName">Last name of the records to seek.</param>
        /// <returns>Array of the found records with the specified lastName.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling FindByLastName() with lastName = {lastName}.");
            IEnumerable<FileCabinetRecord> records = this.service.FindByLastName(lastName);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - FindByLastName() returned {records.Count()} records.");
            return records;
        }

        /// <summary>
        /// Looks for records with dateOfBirth property equal to the specified date parameter.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth of the records to seek.</param>
        /// <returns>Array of the found records with the specified dateOfBirth.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth)
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling FindByDateOfBirth() with dateOfBirth = {dateOfBirth}.");
            IEnumerable<FileCabinetRecord> records = this.service.FindByDateOfBirth(dateOfBirth);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - FindByDateOfBirth() returned {records.Count()} records.");
            return records;
        }

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetDefaultService.
        /// </summary>
        public void ChangeValidatorToCustom()
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling ChangeValidatorToCustom().");
            this.service.ChangeValidatorToCustom();
        }

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetCustomService.
        /// </summary>
        public void ChangeValidatorToDefault()
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling ChangeValidatorToDefault().");
            this.service.ChangeValidatorToDefault();
        }

        /// <summary>
        /// Makes snapshot of the IFileCabinetService with the copy of the records.
        /// </summary>
        /// <returns>FileCabinetServiceSnapshot of the current IFileCabinetService instance.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot()
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling MakeSnapshot().");
            FileCabinetServiceSnapshot snapshot = this.service.MakeSnapshot();
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - MakeSnapshot() returned new snapshot with {snapshot.Records.Count} records.");
            return snapshot;
        }

        /// <summary>
        /// Compares data from snapshot and updates records.
        /// </summary>
        /// <param name="snapshot">Snapshot to compare with.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot)
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling Restore() with snapshot = {snapshot}.");
            this.service.Restore(snapshot);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Restore() completed and {snapshot?.Records.Count} records restored.");
        }

        /// <summary>
        /// Removes record with the specified id.
        /// </summary>
        /// <param name="id">Id of the record to delete.</param>
        public void RemoveRecord(int id)
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling RemoveRecord() with id = {id}.");
            this.service.RemoveRecord(id);
        }

        /// <summary>
        /// Removes deleted records from source database.
        /// </summary>
        public void Purge()
        {
            FileStream fileStream = new (this.path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
            using StreamWriter writer = new (fileStream);
            writer.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm} - Calling Purge().");
            this.service.Purge();
        }
    }
}
