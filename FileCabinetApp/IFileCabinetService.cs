﻿using System.Collections.Immutable;
using FileCabinetApp.Validators;

#pragma warning disable SA1011

namespace FileCabinetApp
{
    /// <summary>
    /// Provides basic structure and methods of the file cabinet.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Gets a value of the date format.
        /// </summary>
        /// <value>dateFormat.</value>
        public string DateFormat { get; }

        /// <summary>
        /// Gets information about the number of records in the service.
        /// </summary>
        /// <value>
        /// <records.Count>Information about the number of records in the service.</records.Count>
        /// </value>
        public (int, int) GetStat
        {
            get;
        }

        /// <summary>
        /// Gets cached search data
        /// </summary>
        /// <value>Copy of the search cache dictionary.</value>
        public Dictionary<ImmutableArray<(string, string)>, IReadOnlyCollection<FileCabinetRecord>> SearchCache
        {
            get;
        }

        /// <summary>
        /// Gets array of validators to validate records for the service.
        /// </summary>
        /// <returns>Array of validators.</returns>
        public IRecordValidator[]? GetValidators();

        /// <summary>
        /// Creates a new record and adds it into the records list.
        /// </summary>
        /// <param name="recordParameters">Parameters of the record to change.</param>
        /// <returns>Id of the created record.</returns>
        public int CreateRecord(FileCabinetRecordParameterObject recordParameters);

        /// <summary>
        /// Creates a new record and adds it into the records list.
        /// </summary>
        /// <param name="id">Id of the record to add.</param>
        /// <param name="recordParameters">Parameters of the record to add.</param>
        /// <returns>Id of the created record.</returns>
        public int CreateRecord(int id, FileCabinetRecordParameterObject recordParameters);

        /// <summary>
        /// Adds cache data to the cache.
        /// </summary>
        public void AddToSearchCache(ImmutableArray<(string, string)> criteria, IReadOnlyCollection<FileCabinetRecord> data);

        /// <summary>
        /// Gets copy of the records as record array.
        /// </summary>
        /// <returns>Array of the records.</returns>
        public IEnumerable<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Edits the existing record with the specified id.
        /// </summary>
        /// <param name="id">Id of the record to edit.</param>
        /// <param name="recordParameters">Parameters of the record to change.</param>
        public void EditRecord(int id, FileCabinetRecordParameterObject recordParameters);

        /// <summary>
        /// Looks for records with firstName property equal to the specified firstName parameter.
        /// </summary>
        /// <param name="firstName">First name of the records to seek.</param>
        /// <returns>Array of the found records with the specified firstName.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Looks for records with lastName property equal to the specified lastName parameter.
        /// </summary>
        /// <param name="lastName">Last name of the records to seek.</param>
        /// <returns>Array of the found records with the specified lastName.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Looks for records with dateOfBirth property equal to the specified date parameter.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth of the records to seek.</param>
        /// <returns>Array of the found records with the specified dateOfBirth.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetDefaultService.
        /// </summary>
        public void ChangeValidatorToCustom();

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetCustomService.
        /// </summary>
        public void ChangeValidatorToDefault();

        /// <summary>
        /// Makes snapshot of the IFileCabinetService with the copy of the records.
        /// </summary>
        /// <returns>FileCabinetServiceSnapshot of the current IFileCabinetService instance.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Compares data from snapshot and updates records.
        /// </summary>
        /// <param name="snapshot">Snapshot to compare with.</param>
        public void Restore(FileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Removes record with the specified id.
        /// </summary>
        /// <param name="id">Id of the record to delete.</param>
        public void RemoveRecord(int id);

        /// <summary>
        /// Removes deleted records from source database.
        /// </summary>
        public void Purge();
    }
}
