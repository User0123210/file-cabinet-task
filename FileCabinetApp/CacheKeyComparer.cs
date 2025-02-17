using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp
{
    /// <summary>
    /// Compares ImmutableArray of (string, string) keys for SearchCache dictionary.
    /// </summary>
    public class CacheKeyComparer : IEqualityComparer<ImmutableArray<(string, string)>>
    {
        /// <summary>
        /// Compares for equality two immutable arrays of (string, string).
        /// </summary>
        /// <param name="first">First array to compare</param>
        /// <param name="second">Second array to compare</param>
        /// <returns>True if equals, otherwise false.</returns>
        public bool Equals(ImmutableArray<(string, string)> first, ImmutableArray<(string, string)> second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }

            var firstHash = new HashSet<(string, string)>(first);
            var secondHash = new HashSet<(string, string)>(second);

            return firstHash.SetEquals(secondHash);
        }

        /// <summary>
        /// Gets hash code of the array.
        /// </summary>
        /// <param name="arr">Array to get hash of.</param>
        /// <returns>Value of the array's hash.</returns>
        public int GetHashCode(ImmutableArray<(string, string)> arr)
        {
            int hash = 17;

            unchecked
            {
                foreach (var (first, second) in arr)
                {
                    hash += (first?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0) ^ (second?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0);
                }
            }

            return hash;
        }
    }
}
