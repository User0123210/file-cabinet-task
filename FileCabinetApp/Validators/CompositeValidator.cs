namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validates record parameter by custom rules.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">Validators.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = validators.ToList();
        }

        /// <summary>
        /// Gets array of validators to validate records in the service.
        /// </summary>
        /// <returns>Array of validators.</returns>
        public IRecordValidator[] GetValidators()
        {
            return this.validators.ToArray();
        }

        /// <summary>
        /// Validates record parameters for creation or editing of a new record.
        /// </summary>
        /// <param name="recordsParameters">Parameters of the record to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when recordsParameters are null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when firstName or lastName is null.</exception>
        /// <exception cref="ArgumentException">Thrown when firstName or lastName less than 2 or more than 60.</exception>
        /// <exception cref="ArgumentException">Thrown when firstName or lastName empty or whitespace.</exception>
        /// <exception cref="ArgumentException">Thrown when dateOfBirth less than "01-01-1950" or more than current date.</exception>
        /// <exception cref="ArgumentException">Thrown when salary less than 0.</exception>
        /// <exception cref="ArgumentException">Thrown when record with the specified id isn't found.</exception>
        public Tuple<bool, string> ValidateParameters(object recordsParameters)
        {
            ArgumentNullException.ThrowIfNull(recordsParameters);
            FileCabinetRecordParameterObject parameters = (recordsParameters as FileCabinetRecordParameterObject) !;

            Tuple<bool, string> validationResult = new (true, string.Empty);

            foreach (var validator in this.validators)
            {
                if (validator.GetType() == typeof(FirstNameValidator))
                {
                    validationResult = validator.ValidateParameters(parameters.FirstName);
                }
                else if (validator.GetType() == typeof(LastNameValidator))
                {
                    validationResult = validator.ValidateParameters(parameters.LastName);
                }
                else if (validator.GetType() == typeof(DateOfBirthValidator))
                {
                    validationResult = validator.ValidateParameters(parameters.DateOfBirth);
                }
                else if (validator.GetType() == typeof(StatusValidator))
                {
                    validationResult = validator.ValidateParameters(parameters.Status);
                }
                else if (validator.GetType() == typeof(SalaryValidator))
                {
                    validationResult = validator.ValidateParameters(parameters.Salary);
                }
                else if (validator.GetType() == typeof(PermissionsValidator))
                {
                    validationResult = validator.ValidateParameters(parameters.Permissions);
                }

                if (!validationResult.Item1)
                {
                    return new Tuple<bool, string>(false, validationResult.Item2);
                }
            }

            return validationResult;
        }
    }
}
