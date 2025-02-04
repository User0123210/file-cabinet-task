using System;
using System.Collections.Generic;
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
        /// Gets array of valid permissions.
        /// </summary>
        /// <returns>An array of valid permissions.</returns>
        public char[] GetValidPermissions();

        /// <summary>
        /// Validates record parameters for creation or editing of a new record.
        /// </summary>
        /// <param name="recordParameters">Parameters of the record to validate.</param>
        public void ValidateParameters(FileCabinetRecordParameterObject? recordParameters);
    }
}
