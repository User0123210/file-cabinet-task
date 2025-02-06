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
    /// Validator of the record parameters.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Gets minimal possible length of the name.
        /// </summary>
        /// <value>minNameLength.</value>
        public int MinNameLength { get; init; }

        /// <summary>
        /// Gets maximum possible length of the name.
        /// </summary>
        /// <value>maxNameLength.</value>
        public int MaxNameLength { get; init; }

        /// <summary>
        /// Gets minimum possible date.
        /// </summary>
        /// <value>minDate.</value>
        public DateTime MinDate { get; init; }

        /// <summary>
        /// Gets a value indicating whether the name should contain only letter characters or not.
        /// </summary>
        /// <value>isOnlyLetterName.</value>
        public bool IsOnlyLetterName { get; init; }

        /// <summary>
        /// Gets a value of the date format.
        /// </summary>
        /// <value>dateFormat.</value>
        public string DateFormat { get; init; }

        /// <summary>
        /// Gets a value indicating minimum salary value.
        /// </summary>
        /// <value>isOnlyLetterName.</value>
        public decimal MinSalaryValue { get; init; }

        /// <summary>
        /// Gets array of valid permissions.
        /// </summary>
        /// <returns>An array of valid permissions.</returns>
        public ReadOnlyCollection<char> GetValidPermissions();

        /// <summary>
        /// Validates record parameters for creation or editing of a new record.
        /// </summary>
        /// <param name="recordParameters">Parameters of the record to validate.</param>
        public void ValidateParameters(FileCabinetRecordParameterObject? recordParameters);

        public Tuple<bool, string> ValidateName(string? name);

        public Tuple<bool, string> ValidateDateOfBirth(DateTime? dateOfBirth);

        public Tuple<bool, string> ValidateStatus(short? status);

        public Tuple<bool, string> ValidateSalary(decimal? salary);

        public Tuple<bool, string> ValidatePermissions(char? permissions);
    }
}
