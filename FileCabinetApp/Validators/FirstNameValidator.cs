using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents class to validate first name in the record.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int minNameLength;
        private readonly int maxNameLength;
        private readonly bool isOnlyCharacters;

        /// <summary>
        /// Initializes new instance of the FirstNameValidator via the minNameLength, maxNameLength, isOnlyCharacters parameters.
        /// </summary>
        /// <param name="minNameLength">Minimum possible length of the name.</param>
        /// <param name="maxNameLength">Maximum possible length of the name.</param>
        /// <param name="isOnlyCharacters">Should name contain only characters or not.</param>
        /// <exception cref="ArgumentException"></exception>
        public FirstNameValidator(int minNameLength, int maxNameLength, bool isOnlyCharacters)
        {
            if (minNameLength > maxNameLength)
            {
                throw new ArgumentException("Minimal name length can't be more than maximum.");
            }

            this.minNameLength = minNameLength;
            this.maxNameLength = maxNameLength;
            this.isOnlyCharacters = isOnlyCharacters;
        }

        /// <summary>
        /// Gets minimal possible length of the recordsParameters.
        /// </summary>
        /// <value>minNameLength.</value>
        public int MinNameLength { get => this.minNameLength; }

        /// <summary>
        /// Gets maximum possible length of the recordsParameters.
        /// </summary>
        /// <value>maxNameLength.</value>
        public int MaxNameLength { get => this.maxNameLength; }

        /// <summary>
        /// Gets a value indicating whether the recordsParameters should contain only letter characters or not.
        /// </summary>
        /// <value>isOnlyLetterName.</value>
        public bool IsOnlyLetterName { get => this.isOnlyCharacters; }

        /// <summary>
        /// Validates parameteres of the record, that consist of first name.
        /// </summary>
        /// <param name="recordsParameters">First name to validate.</param>
        /// <returns>Bool is name valid or note, and string message which requirements name doesn't meet.</returns>
        public Tuple<bool, string> ValidateParameters(object recordsParameters)
        {
            int minLength = this.MinNameLength;
            int maxLength = this.MaxNameLength;

            string parameters = (string)recordsParameters;

            if (string.IsNullOrWhiteSpace(parameters))
            {
                return new Tuple<bool, string>(false, "First name shouldn't be empty or whitespace");
            }

            if (parameters.Length < minLength || parameters.Length > maxLength)
            {
                return new Tuple<bool, string>(false, $"First name's length should be more or equal {minLength} and less or equal {maxLength}");
            }

            if (this.isOnlyCharacters)
            {
                foreach (char character in parameters)
                {
                    if (!char.IsLetter(character))
                    {
                        return new Tuple<bool, string>(false, "First name should contain only letters.");
                    }
                }
            }

            return new Tuple<bool, string>(true, "Everything is alright");
        }
    }
}
