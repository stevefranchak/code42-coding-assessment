using System;

namespace EngineerHomework.Models
{
    public class User
    {
        private static int USER_DATA_TOKENS_PER_LINE = 3;
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

        public int UserId { get; private set; }

        public int OrgId { get; private set; } 

        public int NumFiles { get; private set; }
    }
}
