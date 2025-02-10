using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents class to store parameters for DateOfBirthValidator.
    /// </summary>
    [JsonObject("dateOfBirth")]
    public class DateOfBirthValidatorParameterObject
    {
        /// <summary>
        /// Initializes new instance of DateOfBirthValidatorParameterObject class.
        /// </summary>
        public DateOfBirthValidatorParameterObject()
        {
        }

        /// <summary>
        /// Gets minimum possible date.
        /// </summary>
        /// <value>minDate.</value>
        [JsonProperty("from")]
        public DateTime From { get; set; }

        /// <summary>
        /// Gets minimum possible date.
        /// </summary>
        /// <value>minDate.</value>
        [JsonProperty("to")]
        public DateTime To { get; set; }

        /// <summary>
        /// Gets a value of the date format.
        /// </summary>
        /// <value>dateFormat.</value>
        [JsonProperty("dateFormat")]
        public string DateFormat { get; set; } = "MM/dd/yyyy";
    }
}
