using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Manages information about the records in file cabinet with default parameters validation.
    /// </summary>
    public class FileCabinetDefaultService : FileCabinetService
    {
        private readonly int minNameLength = 2;
        private readonly int maxNameLength = 60;
        private readonly DateTime minDate = new (1950, 1, 1);
        private readonly char[] validPermissions = Array.Empty<char>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetDefaultService"/> class.
        /// </summary>
        public FileCabinetDefaultService()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetDefaultService"/> class.
        /// </summary>
        /// <param name="records">Parameter to assign to records list.</param>
        /// <param name="firstNameDictionary">Parameter to assign to firstNameDictionary dictionary.</param>
        /// <param name="lastNameDictionary">Parameter to assign to lastNameDictionary dictionary.</param>
        /// <param name="dateOfBirthDictionary">Parameter to assign to dateOfBirthDictionary dictionary.</param>
        /// <param name="recordIdDictionary">Parameter to assign to recordIdDictionary dictionary.</param>
        public FileCabinetDefaultService(IList<FileCabinetRecord> records, Dictionary<string, List<FileCabinetRecord>> firstNameDictionary, Dictionary<string, List<FileCabinetRecord>> lastNameDictionary, Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary, Dictionary<int, FileCabinetRecord> recordIdDictionary)
            : base(records, firstNameDictionary, lastNameDictionary, dateOfBirthDictionary, recordIdDictionary)
        {
        }

        /// <summary>
        /// Gets minimal possible length of the name.
        /// </summary>
        /// <value>this.minNameLength.</value>
        public override int MinNameLength { get => this.minNameLength; }

        /// <summary>
        /// Gets maximum possible length of the name.
        /// </summary>
        /// <value>this.maxNameLength.</value>
        public override int MaxNameLength { get => this.maxNameLength; }

        /// <summary>
        /// Gets minimum possible date.
        /// </summary>
        /// <value>this.minDate.</value>
        public override DateTime MinDate { get => this.minDate; }

        /// <summary>
        /// Gets array of valid permissions.
        /// </summary>
        /// <returns>An array of valid permissions.</returns>
        public override char[] GetValidPermissions()
        {
            return (char[])this.validPermissions.Clone();
        }

        /// <summary>
        /// Validates record parameters for creation or editing of a new record.
        /// </summary>
        /// <param name="recordParameters">Parameters of the record to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when recordParameters are null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when firstName or lastName is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firstName or lastName less than 2 or more than 60.</exception>
        /// <exception cref="ArgumentException">Thrown when firstName or lastName empty or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown when dateOfBirth less than "01-01-1950" or more than current date.</exception>
        /// <exception cref="ArgumentException">Thrown when salary less than 0.</exception>
        /// <exception cref="ArgumentException">Thrown when record with the specified id isn't found.</exception>
        protected override void ValidateParameters(FileCabinetRecordParameterObject? recordParameters)
        {
            int minLength = this.MinNameLength;
            int maxLength = this.MaxNameLength;

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

            DateTime minDate = this.MinDate;

            if (recordParameters.DateOfBirth < minDate || recordParameters.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException($"Date of birth shouldn't be less than {minDate} or more than current date.", nameof(recordParameters));
            }

            if (recordParameters.Salary < 0)
            {
                throw new ArgumentException("Salary value shouldn't be less than 0", nameof(recordParameters));
            }
        }
    }
}
