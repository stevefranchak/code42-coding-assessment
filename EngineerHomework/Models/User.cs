using System;

namespace EngineerHomework.Models
{
    public class User
    {
        private const int USER_DATA_TOKENS_PER_LINE = 3;

        /// <summary>
        /// Generates a new <see cref="User"/> instance with fields set to the column values of the provided
        /// <paramref name="csvLine"/>.
        /// </summary>
        ///
        /// <param name="csvLine">String in the format "userId, orgId, numFiles"</param>
        ///
        /// <returns>The generated <see cref="User"/></returns>
        ///
        /// <exception cref="FormatException">Throw when there are not exactly three column values</exception>
        /// <exception cref="FormatException">Throw when a non-int32 value is provided for any of the column values</exception>
        public static User Generate(String csvLine)
        {
            var tokens = csvLine.Split(",");

            if (tokens.Length != USER_DATA_TOKENS_PER_LINE)
            {
                throw new FormatException($"Invalid line found in User data input file: {csvLine}");
            }

            return new User(int.Parse(tokens[0].Trim()), int.Parse(tokens[1].Trim()), int.Parse(tokens[2].Trim()));
        }

        private User(int userId, int orgId, int numFiles)
        {
            UserId = userId;
            OrgId = orgId;
            NumFiles = numFiles;
        }

        /// <summary>This user's ID</summary>
        public int UserId { get; private set; }

        /// <summary>The organization ID for the organization this user directly reports to.</summary>
        public int OrgId { get; private set; }

        /// <summary>The number of files owned by this user.</summary>
        public int NumFiles { get; private set; }
    }
}
