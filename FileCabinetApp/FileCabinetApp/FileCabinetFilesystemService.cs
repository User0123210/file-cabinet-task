using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Represent file cabinet service for work with filesytem.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        /// <summary>
        /// Gets minimal possible length of the name.
        /// </summary>
        /// <value>this.minNameLength.</value>
        public int MinNameLength { get; }

        /// <summary>
        /// Gets maximum possible length of the name.
        /// </summary>
        /// <value>this.maxNameLength.</value>
        public int MaxNameLength { get; }

        /// <summary>
        /// Gets a value indicating whether the name should contain only letter characters or not.
        /// </summary>
        /// <value>isOnlyLetterName.</value>
        public bool IsOnlyLetterName { get; }

        /// <summary>
        /// Gets a value of the date format.
        /// </summary>
        /// <value>dateFormat.</value>
        public string DateFormat { get; } = string.Empty;

        /// <summary>
        /// Gets minimum possible date.
        /// </summary>
        /// <value>this.minDate.</value>
        public DateTime MinDate { get; }

        /// <summary>
        /// Gets information about the number of records in the service.
        /// </summary>
        /// <value>
        /// <records.Count>Information about the number of records in the service.</records.Count>
        /// </value>
        public int GetStat
        {
            get;
        }

        /// <summary>
        /// Gets array of valid permissions.
        /// </summary>
        /// <returns>An array of valid permissions.</returns>
        public ReadOnlyCollection<char> GetValidPermissions() => throw new NotImplementedException();

        /// <summary>
        /// Creates a new record and adds it into the records list.
        /// </summary>
        /// <param name="recordParameters">Parameters of the record to change.</param>
        /// <returns>Id of the created record.</returns>
        public int CreateRecord(FileCabinetRecordParameterObject recordParameters) => throw new NotImplementedException();

        /// <summary>
        /// Gets copy of the records as record array.
        /// </summary>
        /// <returns>Array of the records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords() => throw new NotImplementedException();

        /// <summary>
        /// Edits the existing record with the specified id.
        /// </summary>
        /// <param name="id">Id of the record to edit.</param>
        /// <param name="recordParameters">Parameters of the record to change.</param>
        public void EditRecord(int id, FileCabinetRecordParameterObject recordParameters) => throw new NotImplementedException();

        /// <summary>
        /// Looks for records with firstName property equal to the specified firstName parameter.
        /// </summary>
        /// <param name="firstName">First name of the records to seek.</param>
        /// <returns>Array of the found records with the specified firstName.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName) => throw new NotImplementedException();

        /// <summary>
        /// Looks for records with lastName property equal to the specified lastName parameter.
        /// </summary>
        /// <param name="lastName">Last name of the records to seek.</param>
        /// <returns>Array of the found records with the specified lastName.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName) => throw new NotImplementedException();

        /// <summary>
        /// Looks for records with dateOfBirth property equal to the specified date parameter.
        /// </summary>
        /// <param name="date">Date of birth of the records to seek.</param>
        /// <returns>Array of the found records with the specified dateOfBirth.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(DateTime date) => throw new NotImplementedException();

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetDefaultService.
        /// </summary>
        public void ChangeValidatorToCustom() => throw new NotImplementedException();

        /// <summary>
        /// Creates a copy of the FileCabinetMemoryService as FileCabinetCustomService.
        /// </summary>
        public void ChangeValidatorToDefault() => throw new NotImplementedException();

        /// <summary>
        /// Makes snapshot of the IFileCabinetService with the copy of the records.
        /// </summary>
        /// <returns>FileCabinetServiceSnapshot of the current IFileCabinetService instance.</returns>
        public FileCabinetServiceSnapshot MakeSnapshot() => throw new NotImplementedException();
    }
}
