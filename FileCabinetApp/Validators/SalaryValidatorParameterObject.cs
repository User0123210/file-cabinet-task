using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents class to store parameters for SalaryValidator.
    /// </summary>
    [JsonObject("salary")]
    public class SalaryValidatorParameterObject
    {
        /// <summary>
        /// Initializes new instance of SalaryValidatorParameterObject class.
        /// </summary>
        public SalaryValidatorParameterObject()
        {
        }

        /// <summary>
        /// Gets minimum value of salary.
        /// </summary>
        /// <value>minSalary, that represents value more than which should be salary.</value>
        [JsonProperty("minSalary")]
        public decimal MinSalary { get; set; }
    }
}
