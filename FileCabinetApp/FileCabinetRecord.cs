using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents user's record in the FileCabinet.
    /// </summary>
    [XmlType("record")]
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets Id of the record.
        /// </summary>
        /// <value>
        /// <Id>Id of the record.</Id>
        /// </value>
        [XmlAttribute("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets user's FirstName.
        /// </summary>
        /// <value>
        /// <FirstName>First name in the record.</FirstName>
        /// </value>
        [XmlElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets user's LastName.
        /// </summary>
        /// <value>
        /// <LastName>Last name in the record.</LastName>
        /// </value>
        [XmlElement("lastName")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets user's DateOfBirth.
        /// </summary>
        /// <value>
        /// <DateOfBirth>Date of birth in the record.</DateOfBirth>
        /// </value>
        [XmlElement("dateOfBirth", DataType = "date")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets user's Status.
        /// </summary>
        /// <value>
        /// <Status>Status in the record.</Status>
        /// </value>
        [XmlElement("status")]
        public short Status { get; set; }

        /// <summary>
        /// Gets or sets user's Salary.
        /// </summary>
        /// <value>
        /// <Salary>Salary in the record.</Salary>
        /// </value>
        [XmlElement("salary")]
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets user's Permissions.
        /// </summary>
        /// <value>
        /// <Permissions>Permissions in the record.</Permissions>
        /// </value>
        [XmlElement("permissions")]
        public char Permissions { get; set; }
    }
}
