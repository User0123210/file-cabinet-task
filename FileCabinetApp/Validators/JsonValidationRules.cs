using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents class to store deserialized validation rules from json file.
    /// </summary>
    public class JsonValidationRules
    {
        /// <summary>
        /// Initializes new instance of JsonValidationRules class.
        /// </summary>
        public JsonValidationRules()
        {
        }

        /// <summary>
        /// Contains parameters for the FirstNameValidator.
        /// </summary>
        /// <value>Parameters for validation rules of first name validator.</value>
        [JsonProperty("firstName")]
        public FirstNameValidatorParameterObject FirstName { get; set; } = new FirstNameValidatorParameterObject();

        /// <summary>
        /// Contains parameters for the LastNameValidator.
        /// </summary>
        /// <value>Parameters for validation rules of last name validator.</value>
        [JsonProperty("lastName")]
        public LastNameValidatorParameterObject LastName { get; set; } = new LastNameValidatorParameterObject();

        /// <summary>
        /// Contains parameters for the DateOfBirthValidator.
        /// </summary>
        /// <value>Parameters for validation rules of date of birth validator.</value>
        [JsonProperty("dateOfBirth")]
        public DateOfBirthValidatorParameterObject DateOfBirth { get; set; } = new DateOfBirthValidatorParameterObject();

        /// <summary>
        /// Contains parameters for the SalaryValidator.
        /// </summary>
        /// <value>Parameters for validation rules of salary validator.</value>
        [JsonProperty("salary")]
        public SalaryValidatorParameterObject Salary { get; set; } = new SalaryValidatorParameterObject();

        /// <summary>
        /// Contains parameters for the PermissionsValidator.
        /// </summary>
        /// <value>Parameters for validation rules of permissions validator.</value>
        [JsonProperty("permissions")]
        public PermissionsValidatorParameterObject Permissions { get; set; } = new PermissionsValidatorParameterObject();
    }
}
