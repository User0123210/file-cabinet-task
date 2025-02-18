namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents salary validator for the record.
    /// </summary>
    public class SalaryValidator : IRecordValidator
    {
        private readonly decimal minSalary;

        /// <summary>
        /// Initializes new instance of SalaryValidator class via the specified minimum salary.
        /// </summary>
        /// <param name="minSalary">Parameter, that represents minimal salary.</param>
        public SalaryValidator(decimal minSalary)
        {
            this.minSalary = minSalary;
        }

        /// <summary>
        /// Gets minimum value of salary.
        /// </summary>
        /// <value>minSalary, that represents value more than which should be salary.</value>
        public decimal MinSalary
        {
            get => this.minSalary;
        }

        /// <summary>
        /// Validates parameters of the records, that consis of salary value.
        /// </summary>
        /// <param name="recordsParameters"></param>
        /// <returns></returns>
        public Tuple<bool, string> ValidateParameters(object recordsParameters)
        {
            return (decimal)recordsParameters < this.MinSalary ? new Tuple<bool, string>(false, "Salary value shouldn't be less than 0") : new Tuple<bool, string>(true, "Everything is alright");
        }
    }
}
