using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Validates record parameter by default rules.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        private readonly char[] validPermissions = Array.Empty<char>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidator"/> class.
        /// </summary>
        public DefaultValidator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidator"/> class.
        /// </summary>
        /// <param name="minNameLength">Minimal name length.</param>
        /// <param name="maxNameLength">Maximum name length.</param>
        /// <param name="minDate">Minimum date.</param>
        public DefaultValidator(int minNameLength, int maxNameLength, DateTime minDate)
        {
            this.MinNameLength = minNameLength;
            this.MaxNameLength = maxNameLength;
            this.MinDate = minDate;
        }

        /// <summary>
        /// Gets minimal possible length of the name.
        /// </summary>
        /// <value>minNameLength.</value>
        public int MinNameLength { get; init; } = 2;

        /// <summary>
        /// Gets maximum possible length of the name.
        /// </summary>
        /// <value>maxNameLength.</value>
        public int MaxNameLength { get; init; } = 60;

        /// <summary>
        /// Gets minimum possible date.
        /// </summary>
        /// <value>minDate.</value>
        public DateTime MinDate { get; init; } = new (1950, 1, 1);

        /// <summary>
        /// Gets array of valid permissions.
        /// </summary>
        /// <returns>An array of valid permissions.</returns>
        public ReadOnlyCollection<char> GetValidPermissions()
        {
            return new ReadOnlyCollection<char>(this.validPermissions);
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
        public void ValidateParameters(FileCabinetRecordParameterObject? recordParameters)
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
