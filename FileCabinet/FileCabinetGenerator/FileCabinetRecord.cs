using System.Xml;
using System.Xml.Serialization;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents user's record in the FileCabinet.
    /// </summary>
    [XmlRoot("record")]
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
        [XmlElement("firstName", Order = 1)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets user's LastName.
        /// </summary>
        /// <value>
        /// <LastName>Last name in the record.</LastName>
        /// </value>
        [XmlElement("lastName", Order = 2)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets user's DateOfBirth.
        /// </summary>
        /// <value>
        /// <DateOfBirth>Date of birth in the record.</DateOfBirth>
        /// </value>
        [XmlElement("dateOfBirth", Order = 3)]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets user's Status.
        /// </summary>
        /// <value>
        /// <Status>Status in the record.</Status>
        /// </value>
        [XmlElement("status", Order = 4)]
        public short Status { get; set; }

        /// <summary>
        /// Gets or sets user's Salary.
        /// </summary>
        /// <value>
        /// <Salary>Salary in the record.</Salary>
        /// </value>
        [XmlElement("salary", Order = 5)]
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets user's Permissions.
        /// </summary>
        /// <value>
        /// <Permissions>Permissions in the record.</Permissions>
        /// </value>
        [XmlElement("permissions", Order = 6)]
        public char Permissions { get; set; }
    }
}
