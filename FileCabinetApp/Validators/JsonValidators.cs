using Newtonsoft.Json;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents class that stores different json validation rules for different validators.
    /// </summary>
    public class JsonValidators
    {
        /// <summary>
        /// Initializes new instance of JsonValidators class.
        /// </summary>
        public JsonValidators()
        {
        }

        /// <summary>
        /// Gets default validation rules.
        /// </summary>
        /// <value>Rules for the default validator.</value>
        [JsonProperty("default")]
        public JsonValidationRules Default { get; set; } = new JsonValidationRules();

        /// <summary>
        /// Gets custom validation rules.
        /// </summary>
        /// <value>Rules for the custom validator.</value>
        [JsonProperty("custom")]
        public JsonValidationRules Custom { get; set; } = new JsonValidationRules();
    }
}
