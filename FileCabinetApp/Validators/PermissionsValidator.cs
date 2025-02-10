using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents class to validate permissions in the record.
    /// </summary>
    public class PermissionsValidator : IRecordValidator
    {
        private readonly char[] validPermissions;

        /// <summary>
        /// Initializes new instance of PermissionsValidator via the specified array of valid permissions.
        /// </summary>
        /// <param name="validPermissions"></param>
        public PermissionsValidator(char[] validPermissions)
        {
            this.validPermissions = validPermissions;
        }

        /// <summary>
        /// Gets array of valid permissions.
        /// </summary>
        /// <returns>An array of valid permissions.</returns>
        /// <value>
        /// <validPermissions>Array of valid permissions.</validPermissions>
        /// </value>
        public ReadOnlyCollection<char> ValidPermissions
        {
            get => new (this.validPermissions);
        }

        /// <summary>
        /// Validates record parameters, that consist of permissions.
        /// </summary>
        /// <param name="recordsParameters">Permissions to validate.</param>
        /// <returns></returns>
        public Tuple<bool, string> ValidateParameters(object recordsParameters)
        {
            bool isOneOfValidPermissions = false;
            char parameters = (char)recordsParameters;

            foreach (char permission in this.validPermissions)
            {
                if (string.Equals(parameters.ToString(), permission.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    isOneOfValidPermissions = true;
                    break;
                }
            }

            if (!isOneOfValidPermissions)
            {
                return new Tuple<bool, string>(false, $"Permissions should be one of {string.Join(", ", this.ValidPermissions)}");
            }

            return new Tuple<bool, string>(true, "Everything is alright");
        }
    }
}
