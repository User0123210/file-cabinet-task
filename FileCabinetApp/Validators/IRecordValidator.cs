namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validator of the record parameters.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates record parameters for creation or editing of a new record.
        /// </summary>
        /// <param name="recordsParameters">Parameters of the record to validate.</param>
        public Tuple<bool, string> ValidateParameters(object recordsParameters);
    }
}
