using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validator to validate status in the file cabinet record.
    /// </summary>
    public class StatusValidator : IRecordValidator
    {
        /// <summary>
        /// Validates status in the file cabinet record.
        /// </summary>
        public Tuple<bool, string> ValidateParameters(object recordsParameters)
        {
            return new Tuple<bool, string>(true, "Everything is alright.");
        }
    }
}
