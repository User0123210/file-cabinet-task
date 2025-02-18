namespace FileCabinetApp
{
    /// <summary>
    /// Represents parameter object of the record.
    /// </summary>
    public class FileCabinetRecordParameterObject
    {
        /// <summary>
        /// Gets user's FirstName.
        /// </summary>
        /// <value>
        /// <FirstName>First name in the record.</FirstName>
        /// </value>
        public string FirstName { get; init; } = string.Empty;

        /// <summary>
        /// Gets user's LastName.
        /// </summary>
        /// <value>
        /// <LastName>Last name in the record.</LastName>
        /// </value>
        public string LastName { get; init; } = string.Empty;

        /// <summary>
        /// Gets user's DateOfBirth.
        /// </summary>
        /// <value>
        /// <DateOfBirth>Date of birth in the record.</DateOfBirth>
        /// </value>
        public DateTime DateOfBirth { get; init; }

        /// <summary>
        /// Gets user's Status.
        /// </summary>
        /// <value>
        /// <Status>Status in the record.</Status>
        /// </value>
        public short Status { get; init; }

        /// <summary>
        /// Gets user's Salary.
        /// </summary>
        /// <value>
        /// <Salary>Salary in the record.</Salary>
        /// </value>
        public decimal Salary { get; init; }

        /// <summary>
        /// Gets user's Permissions.
        /// </summary>
        /// <value>
        /// <Permissions>Permissions in the record.</Permissions>
        /// </value>
        public char Permissions { get; init; }
    }
}
