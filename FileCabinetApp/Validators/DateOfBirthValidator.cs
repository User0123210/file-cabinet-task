using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents class to validate date of birth of the record.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly DateTime from;
        private readonly DateTime to;

        /// <summary>
        /// Initializes new instance of the DtaeOfBirthValidator via the from, to, dateFormat parameters.
        /// </summary>
        /// <param name="from">Minimum possible date.</param>
        /// <param name="to">Maximum possible date.</param>
        /// <param name="dateFormat">Format of the date.</param>
        /// <exception cref="ArgumentException">Thrown when to less than from.</exception>
        public DateOfBirthValidator(DateTime from, DateTime to, string dateFormat)
        {
            if (from > to)
            {
                throw new ArgumentException("Minimum date can't be more than maximum date.");
            }

            this.from = from;
            this.to = to;
            this.DateFormat = dateFormat;
        }

        /// <summary>
        /// Gets minimum possible date.
        /// </summary>
        /// <value>minDate.</value>
        public DateTime From { get => this.from; }

        /// <summary>
        /// Gets minimum possible date.
        /// </summary>
        /// <value>minDate.</value>
        public DateTime To { get => this.to; }

        /// <summary>
        /// Gets a value of the date format.
        /// </summary>
        /// <value>dateFormat.</value>
        public string DateFormat { get; init; }

        /// <summary>
        /// Validates parameters of the record, that consist of date of birth parameter.
        /// </summary>
        /// <param name="recordsParameters">Date of birth to validate.</param>
        /// <returns>Bool if date of birth is valid or not, and string message if date of birth doesn't meet some requirement.</returns>
        public Tuple<bool, string> ValidateParameters(object recordsParameters)
        {
            DateTime date = (DateTime)recordsParameters;

            if (date < this.From || date > DateTime.Now)
            {
                return new Tuple<bool, string>(false, $"Date of birth shouldn't be less than {this.From} or more than {this.To}.");
            }

            return new Tuple<bool, string>(true, "Everything is alright");
        }
    }
}
