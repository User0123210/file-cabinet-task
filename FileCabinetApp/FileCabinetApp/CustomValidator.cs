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
    /// Validates record parameter by custom rules.
    /// </summary>
    public class CustomValidator : IRecordValidator
    {
        private readonly char[] validPermissions = { 'r', 'w', 'x', 'd', 'c', 'm', 'f', 'l', 'a' };

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        public CustomValidator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        /// <param name="minNameLength">Minimal name length.</param>
        /// <param name="maxNameLength">Maximum name length.</param>
        /// <param name="minDate">Minimum date.</param>
        /// <param name="validPermissions">Array of valid permissions characters.</param>
        public CustomValidator(int minNameLength, int maxNameLength, DateTime minDate, char[] validPermissions)
        {
            this.MinNameLength = minNameLength;
            this.MaxNameLength = maxNameLength;
            this.MinDate = minDate;
            this.validPermissions = validPermissions;
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
        public int MaxNameLength { get; init; } = 35;

        /// <summary>
        /// Gets minimum possible date.
        /// </summary>
        /// <value>minDate.</value>
        public DateTime MinDate { get; init; } = new (1908, 6, 8);

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
            ArgumentNullException.ThrowIfNull(recordParameters);
            ArgumentNullException.ThrowIfNull(recordParameters.FirstName);

            if (string.IsNullOrWhiteSpace(recordParameters.FirstName))
            {
                throw new ArgumentException("First name shouldn't be empty or whitespace", nameof(recordParameters));
            }

            if (recordParameters.FirstName.Length < this.MinNameLength || recordParameters.FirstName.Length > this.MaxNameLength)
            {
                throw new ArgumentException($"First name's length should be more or equal {this.MinNameLength} and less or equal {this.MaxNameLength}", nameof(recordParameters));
            }

            foreach (char character in recordParameters.FirstName)
            {
                if (!char.IsLetter(character))
                {
                    throw new ArgumentException("First name should contain letters only", nameof(recordParameters));
                }
            }

            ArgumentNullException.ThrowIfNull(recordParameters.LastName);

            if (string.IsNullOrWhiteSpace(recordParameters.LastName))
            {
                throw new ArgumentException("Last name shouldn't be empty or whitespace", nameof(recordParameters));
            }

            if (recordParameters.LastName.Length < this.MinNameLength || recordParameters.LastName.Length > this.MaxNameLength)
            {
                throw new ArgumentException($"Last name's length should be more or equal {this.MinNameLength} and less or equal {this.MaxNameLength}", nameof(recordParameters));
            }

            foreach (char character in recordParameters.LastName)
            {
                if (!char.IsLetter(character))
                {
                    throw new ArgumentException("Last name should contain letters only", nameof(recordParameters));
                }
            }

            if (recordParameters.DateOfBirth < this.MinDate || recordParameters.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException($"Date of birth shouldn't be less than {this.MinDate} or more than current date.", nameof(recordParameters));
            }

            if (recordParameters.Salary < 0)
            {
                throw new ArgumentException("Salary value shouldn't be less than 0", nameof(recordParameters));
            }

            bool isOneOfValidPermissions = false;

            foreach (char permission in this.validPermissions)
            {
                if (char.Equals(char.ToLowerInvariant(recordParameters.Permissions), permission))
                {
                    isOneOfValidPermissions = true;
                    break;
                }
            }

            if (!isOneOfValidPermissions)
            {
                throw new ArgumentException($"Permissions should be one of {this.validPermissions}.");
            }
        }
    }
}
