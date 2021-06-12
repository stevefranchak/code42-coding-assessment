using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EngineerHomework.Models
{
    public class Org
    {
        public static int ROOT_ORG_PARENT_ORG_ID = 0;
        private static int ORG_DATA_TOKENS_PER_LINE = 3;

        private Org(int parentId, int id, string name)
        {
            ParentId = parentId;
            Id = id;
            Name = name;
        }

        public static Org Generate(string csvLine)
        {
            var tokens = csvLine.Split(",");

            if (tokens.Length != ORG_DATA_TOKENS_PER_LINE)
            {
                throw new FormatException($"Invalid line found in Org hierarchy input file: {csvLine}");
            }

            int orgId = int.Parse(tokens[0].Trim());
            int parentOrgId;

            try
            {
                parentOrgId = int.Parse(tokens[1].Trim());
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException)
            {
                // This is where we catch 'null' parent Org ID
                parentOrgId = ROOT_ORG_PARENT_ORG_ID;
            }

            var orgName = tokens[2].Trim();

            return new Org(parentOrgId, orgId, orgName);
        }

        public int ParentId { get; private set; }

        public int Id { get; private set; }

        public string Name { get; private set; }
    }
}
