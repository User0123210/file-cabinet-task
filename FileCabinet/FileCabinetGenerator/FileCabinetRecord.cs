using System;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents user's record in the FileCabinet.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets Id of the record.
        /// </summary>
        /// <value>
        /// <Id>Id of the record.</Id>
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets user's FirstName.
        /// </summary>
        /// <value>
        /// <FirstName>First name in the record.</FirstName>
        /// </value>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets user's LastName.
        /// </summary>
        /// <value>
        /// <LastName>Last name in the record.</LastName>
        /// </value>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets user's DateOfBirth.
        /// </summary>
        /// <value>
        /// <DateOfBirth>Date of birth in the record.</DateOfBirth>
        /// </value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets user's Status.
        /// </summary>
        /// <value>
        /// <Status>Status in the record.</Status>
        /// </value>
        public short Status { get; set; }

        /// <summary>
        /// Gets or sets user's Salary.
        /// </summary>
        /// <value>
        /// <Salary>Salary in the record.</Salary>
        /// </value>
        public decimal Salary { get; set; }

        /// <summary>
        /// Gets or sets user's Permissions.
        /// </summary>
        /// <value>
        /// <Permissions>Permissions in the record.</Permissions>
        /// </value>
        public char Permissions { get; set; }
    }
}
