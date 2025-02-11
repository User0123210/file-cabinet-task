using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Newtonsoft.Json;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Builds new instance of composite validator via the specified validators.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new ();

        /// <summary>
        /// Initializes new instance of ValidatorBuilder class.
        /// </summary>
        public ValidatorBuilder()
        {
        }

        /// <summary>
        /// Creates new composite validator using list of validators.
        /// </summary>
        /// <returns>New instance of composite validator.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }

        /// <summary>
        /// Adds validator for the first name in the list of validators.
        /// </summary>
        public void ValidateFirstName(int minLength, int maxLength, bool isOnlyLetters)
        {
            IRecordValidator? validator = new FirstNameValidator(minLength, maxLength, isOnlyLetters) as IRecordValidator;

            this.validators.Add(validator);
        }

        /// <summary>
        /// Adds validator for the last name in the list of validators.
        /// </summary>
        public void ValidateLastName(int minLength, int maxLength, bool isOnlyLetters)
        {
            IRecordValidator? validator = new LastNameValidator(minLength, maxLength, isOnlyLetters) as IRecordValidator;

            this.validators.Add(validator);
        }

        /// <summary>
        /// Adds validator for the date of birth in the list of validators.
        /// </summary>
        public void ValidateDateOfBirth(DateTime from, DateTime to, string dateFormat)
        {
            IRecordValidator? validator = new DateOfBirthValidator(from, to, dateFormat) as IRecordValidator;

            this.validators.Add(validator);
        }

        /// <summary>
        /// Adds validator for the status in the list of validators.
        /// </summary>
        public void ValidateStatus()
        {
            IRecordValidator? validator = new StatusValidator() as IRecordValidator;

            this.validators.Add(validator);
        }

        /// <summary>
        /// Adds validator for the salary in the list of validators.
        /// </summary>
        public void ValidateSalary(decimal minSalary)
        {
            IRecordValidator? validator = new SalaryValidator(minSalary) as IRecordValidator;

            this.validators.Add(validator);
        }

        /// <summary>
        /// Adds validator for the permissions in the list of validators.
        /// </summary>
        public void ValidatePermissions(char[] validPermissions)
        {
            IRecordValidator validator = new PermissionsValidator(validPermissions);

            this.validators.Add(validator);
        }

        /// <summary>
        /// Adds validators for the default composite validator in the list of validators.
        /// </summary>
        public IRecordValidator CreateDefault()
        {
            string json = File.ReadAllText("validation-rules.json");

            if (json is not null)
            {
                JsonValidators parameters = JsonConvert.DeserializeObject<JsonValidators>(json) ?? new JsonValidators();
                this.ValidateFirstName(parameters.Default.FirstName.MinNameLength, parameters.Default.FirstName.MaxNameLength, parameters.Default.FirstName.IsOnlyLetters);
                this.ValidateLastName(parameters.Default.LastName.MinNameLength, parameters.Default.LastName.MaxNameLength, parameters.Default.LastName.IsOnlyLetters);
                this.ValidateDateOfBirth(parameters.Default.DateOfBirth.From, parameters.Default.DateOfBirth.To, parameters.Default.DateOfBirth.DateFormat);
                this.ValidateSalary(parameters.Default.Salary.MinSalary);
            }

            return this.Create();
        }

        /// <summary>
        /// Adds validator for the custom composite validator in the list of validators.
        /// </summary>
        public IRecordValidator CreateCustom()
        {
            string json = File.ReadAllText("validation-rules.json");

            if (json is not null)
            {
                JsonValidators parameters = JsonConvert.DeserializeObject<JsonValidators>(json) ?? new JsonValidators();
                this.ValidateFirstName(parameters.Custom.FirstName.MinNameLength, parameters.Custom.FirstName.MaxNameLength, parameters.Custom.FirstName.IsOnlyLetters);
                this.ValidateLastName(parameters.Custom.LastName.MinNameLength, parameters.Custom.LastName.MaxNameLength, parameters.Custom.LastName.IsOnlyLetters);
                this.ValidateDateOfBirth(parameters.Custom.DateOfBirth.From, parameters.Custom.DateOfBirth.To, parameters.Custom.DateOfBirth.DateFormat);
                this.ValidateSalary(parameters.Custom.Salary.MinSalary);
                this.ValidatePermissions(parameters.Custom.Permissions.ValidPermissions.ToArray());
            }

            return this.Create();
        }
    }
}
