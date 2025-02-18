namespace FileCabinetApp
{
    /// <summary>
    /// Compares FileCabinetRecord instances.
    /// </summary>
    internal class FileCabinetRecordComparer : IEqualityComparer<FileCabinetRecord>
    {
        /// <summary>
        /// Compares for equality two FileCabinetRecord instances.
        /// </summary>
        /// <param name="first">First instance to compare</param>
        /// <param name="second">Second instance to compare</param>
        /// <returns>True if equals, otherwise false.</returns>
        public bool Equals(FileCabinetRecord? first, FileCabinetRecord? second)
        {
            if (first is null && second is null)
            {
                return true;
            }
            else if ((first is null && second is not null) || (first is not null && second is null))
            {
                return false;
            }

            if (first!.Id != second!.Id)
            {
                return false;
            }

            return first.FirstName == second.FirstName && first.LastName == second.LastName && first.DateOfBirth == second.DateOfBirth && first.Status == second.Status && first.Salary == second.Salary && first.Permissions == second.Permissions;
        }

        /// <summary>
        /// Gets hash code of the FileCabinetRecord.
        /// </summary>
        /// <param name="rec">Record to get hash of.</param>
        /// <returns>Value of the array's hash.</returns>
        public int GetHashCode(FileCabinetRecord rec)
        {
            int hash = 17;

            unchecked
            {
                hash += rec.Id.GetHashCode();
                hash *= 23;
                hash += rec.FirstName.GetHashCode(StringComparison.OrdinalIgnoreCase);
                hash *= 23;
                hash += rec.LastName.GetHashCode(StringComparison.OrdinalIgnoreCase);
                hash *= 23;
                hash += rec.DateOfBirth.GetHashCode();
                hash *= 23;
                hash += rec.Status.GetHashCode();
                hash *= 23;
                hash += rec.Salary.GetHashCode();
                hash *= 23;
                hash += rec.Permissions.GetHashCode();
                hash *= 23;
            }

            return hash;
        }
    }
}
