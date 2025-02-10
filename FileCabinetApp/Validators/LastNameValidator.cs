using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents class to validate last name of the record.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private readonly int minNameLength;
        private readonly int maxNameLength;
        private readonly bool isOnlyCharacters;

        /// <summary>
        /// Initializes new instance of the LastNameValidator via the minNameLength, maxNameLength, isOnlyCharacetrs parameters.
        /// </summary>
        /// <param name="minNameLength">Minimum possible length of the name.</param>
        /// <param name="maxNameLength">Maximum possible length of the name.</param>
        /// <param name="isOnlyCharacters">Should name contain only characterrs or not.</param>
        public LastNameValidator(int minNameLength, int maxNameLength, bool isOnlyCharacters)
        {
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
        /// Validates parametres of the record, that contains last name value.
        /// </summary>
        /// <param name="recordsParameters">Last name to validate.</param>
        /// <returns>Bool is name valid or not, and string with the message which requieremnt name doesn't meet.</returns>
        public Tuple<bool, string> ValidateParameters(object recordsParameters)
        {
            int minLength = this.MinNameLength;
            int maxLength = this.MaxNameLength;

            string parameters = (string)recordsParameters;

            if (string.IsNullOrWhiteSpace(parameters))
            {
                return new Tuple<bool, string>(false, "Last name shouldn't be empty or whitespace");
            }

            if (parameters.Length < minLength || parameters.Length > maxLength)
            {
                return new Tuple<bool, string>(false, $"Last name's length should be more or equal {minLength} and less or equal {maxLength}");
            }

            if (this.isOnlyCharacters)
            {
                foreach (char character in parameters)
                {
                    if (!char.IsLetter(character))
                    {
                        return new Tuple<bool, string>(false, "Last name should contain only letters.");
                    }
                }
            }

            return new Tuple<bool, string>(true, "Everything is alright");
        }
    }
}
