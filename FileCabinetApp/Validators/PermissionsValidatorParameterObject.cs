using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Represents class to store parameters for PermissionsValidator.
    /// </summary>
    [JsonObject("permissions")]
    public class PermissionsValidatorParameterObject
    {
        /// <summary>
        /// Initializes new instance of PermissionsValidatorParameterObject class.
        /// </summary>
        public PermissionsValidatorParameterObject()
        {
        }

        /// <summary>
        /// Gets array of valid permissions.
        /// </summary>
        /// <returns>An array of valid permissions.</returns>
        /// <value>
        /// <validPermissions>Array of valid permissions.</validPermissions>
        /// </value>
        [JsonProperty("validPermissions")]
        public ReadOnlyCollection<char> ValidPermissions { get; set; } = new ReadOnlyCollection<char>(Array.Empty<char>());
    }
}
