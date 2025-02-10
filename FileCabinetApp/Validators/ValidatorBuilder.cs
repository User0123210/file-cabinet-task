using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.ValidateFirstName(2, 60, false);
            this.ValidateLastName(2, 60, false);
            this.ValidateDateOfBirth(new DateTime(1950, 1, 1), DateTime.Now, "MM/dd/yyyy");
            this.ValidateSalary(0);
            return this.Create();
        }

        /// <summary>
        /// Adds validator for the custom composite validator in the list of validators.
        /// </summary>
        public IRecordValidator CreateCustom()
        {
            this.ValidateFirstName(2, 35, true);
            this.ValidateLastName(2, 35, true);
            this.ValidateDateOfBirth(new DateTime(1908, 6, 8), DateTime.Now, "MM/dd/yyyy");
            this.ValidateSalary(0);
            this.ValidatePermissions(new char[] { 'r', 'w', 'x', 'd', 'c', 'm', 'f', 'l', 'a' });
            return this.Create();
        }
    }
}
