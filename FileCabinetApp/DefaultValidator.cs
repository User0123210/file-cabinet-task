using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
        public DefaultValidator(int minNameLength, int maxNameLength, DateTime minDate, decimal minSalaryValue)
        {
            this.MinNameLength = minNameLength;
            this.MaxNameLength = maxNameLength;
            this.MinDate = minDate;
            this.MinSalaryValue = minSalaryValue;
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
        /// Gets a value indicating whether the name should contain only letter characters or not.
        /// </summary>
        /// <value>isOnlyLetterName.</value>
        public bool IsOnlyLetterName { get; init; }

        /// <summary>
        /// Gets a value of the date format.
        /// </summary>
        /// <value>dateFormat.</value>
        public string DateFormat { get; init; } = "MM/dd/yyyy";

        /// <summary>
        /// Gets a value indicating minimum salary value.
        /// </summary>
        /// <value>isOnlyLetterName.</value>
        public decimal MinSalaryValue { get; init; } = 0;

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
            ArgumentNullException.ThrowIfNull(recordParameters.LastName);

            Tuple<bool, string> validationResult;

            Func<object, Tuple<bool, string>>[] validationMethods = new Func<object, Tuple<bool, string>>[] { p => this.ValidateName(p as string), p => this.ValidateName(p as string), p => this.ValidateDateOfBirth(p as DateTime?), p => this.ValidateSalary(p as decimal?) };
            object[] parameters = new object[] { recordParameters.FirstName, recordParameters.LastName, recordParameters.DateOfBirth, recordParameters.Salary };

            for (int i = 0; i < validationMethods.Length; i++)
            {
                validationResult = validationMethods[i](parameters[i]);

                if (!validationResult.Item1)
                {
                    throw new ArgumentException(validationResult.Item2, nameof(recordParameters));
                }
            }
        }

        public Tuple<bool, string> ValidateName(string? name)
        {
            int minLength = this.MinNameLength;
            int maxLength = this.MaxNameLength;

            if (string.IsNullOrWhiteSpace(name))
            {
                return new Tuple<bool, string>(false, "Name shouldn't be empty or whitespace");
            }

            if (name.Length < minLength || name.Length > maxLength)
            {
                return new Tuple<bool, string>(false, $"Name's length should be more or equal {minLength} and less or equal {maxLength}");
            }

            return new Tuple<bool, string>(true, name);
        }

        public Tuple<bool, string> ValidateDateOfBirth(DateTime? dateOfBirth)
        {
            if (dateOfBirth < this.MinDate || dateOfBirth > DateTime.Now)
            {
                return new Tuple<bool, string>(false, $"Date of birth shouldn't be less than {this.MinDate} or more than current date.");
            }

            return new Tuple<bool, string>(true, "Everything is alright");
        }

        public Tuple<bool, string> ValidateStatus(short? status)
        {
            return new Tuple<bool, string>(true, "Everything is alright");
        }

        public Tuple<bool, string> ValidateSalary(decimal? salary)
        {
            return salary < this.MinSalaryValue ? new Tuple<bool, string>(false, "Salary value shouldn't be less than 0") : new Tuple<bool, string>(true, "Everything is alright");
        }

        public Tuple<bool, string> ValidatePermissions(char? permissions)
        {
            return new Tuple<bool, string>(true, "Everything is alright");
        }
    }
}
