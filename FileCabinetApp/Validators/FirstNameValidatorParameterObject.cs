using Newtonsoft.Json;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents class to store parameters for FirstNameValidator.
    /// </summary>
    [JsonObject("firstName")]
    public class FirstNameValidatorParameterObject
    {
        /// <summary>
        /// Initializes new instance of FirstNameParameterObject class.
        /// </summary>
        public FirstNameValidatorParameterObject()
        {
        }

        /// <summary>
        /// Gets minimal possible length of the first name.
        /// </summary>
        /// <value>minNameLength.</value>
        [JsonProperty("min")]
        public int MinNameLength { get; set; }

        /// <summary>
        /// Gets maximum possible length of the first name.
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
