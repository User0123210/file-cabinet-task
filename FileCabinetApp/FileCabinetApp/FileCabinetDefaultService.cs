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
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetDefaultService"/> class.
        /// </summary>
        public FileCabinetDefaultService()
            : base(new DefaultValidator())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetDefaultService"/> class.
        /// </summary>
        /// <param name="validator">Parameter of validator to use.</param>
        public FileCabinetDefaultService(IRecordValidator validator)
            : base(validator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetDefaultService"/> class.
        /// </summary>
        /// <param name="validator">Parameter of validator to use.</param>
        /// <param name="records">Parameter to assign to records list.</param>
        /// <param name="firstNameDictionary">Parameter to assign to firstNameDictionary dictionary.</param>
        /// <param name="lastNameDictionary">Parameter to assign to lastNameDictionary dictionary.</param>
        /// <param name="dateOfBirthDictionary">Parameter to assign to dateOfBirthDictionary dictionary.</param>
        /// <param name="recordIdDictionary">Parameter to assign to recordIdDictionary dictionary.</param>
        public FileCabinetDefaultService(IRecordValidator validator, IList<FileCabinetRecord> records, Dictionary<string, List<FileCabinetRecord>> firstNameDictionary, Dictionary<string, List<FileCabinetRecord>> lastNameDictionary, Dictionary<DateTime, List<FileCabinetRecord>> dateOfBirthDictionary, Dictionary<int, FileCabinetRecord> recordIdDictionary)
            : base(validator, records, firstNameDictionary, lastNameDictionary, dateOfBirthDictionary, recordIdDictionary)
        {
        }
    }
}
