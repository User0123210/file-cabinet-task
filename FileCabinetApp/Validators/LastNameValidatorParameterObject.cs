using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents class to store parameters for LastNameValidator.
    /// </summary>
    [JsonObject("lastName")]
    public class LastNameValidatorParameterObject
    {
        /// <summary>
        /// Initializes new instance of LastNameValidatorParameterObject class.
        /// </summary>
        public LastNameValidatorParameterObject()
        {
        }

        /// <summary>
        /// Gets minimal possible length of the last name.
        /// </summary>
        /// <value>minNameLength.</value>
        [JsonProperty("min")]
        public int MinNameLength { get; set; }

        /// <summary>
        /// Gets maximum possible length of the last name.
        /// </summary>
        /// <value>maxNameLength.</value>
        [JsonProperty("max")]
        public int MaxNameLength { get; set; }

        /// <summary>
        /// Gets a value indicating whether the last name should contain only letter characters or not.
        /// </summary>
        /// <value>isOnlyLetterName.</value>
        [JsonProperty("isOnlyLetters")]
        public bool IsOnlyLetters { get; set; }
    }
}
