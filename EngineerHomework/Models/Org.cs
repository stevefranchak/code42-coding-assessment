using System;
using System.Collections.Generic;
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
            catch (Exception exc) when (exc is FormatException || exc is ArgumentNullException)
            {
                // This is where we catch 'null' parent Org ID
                parentOrgId = ROOT_ORG_PARENT_ORG_ID;
            }

            var orgName = tokens[2].Trim();

            return new Org(parentOrgId, orgId, orgName);
        }

        public int GetTotalNumUsers()
        {
            return _numUsers + (
                from childOrg in GetChildOrgs()
                select childOrg.GetTotalNumUsers()
            ).Sum();
        }

        public int GetTotalNumFiles()
        {
            return _numFiles + (
                from childOrg in GetChildOrgs()
                select childOrg.GetTotalNumFiles()
            ).Sum();
        }

        public List<Org> GetChildOrgs()
        {
            return _childOrgs.ToList();
        }

        public void AddChildOrg(Org org)
        {
            if (org.ParentId != Id)
            {
                throw new ArgumentException($"Org {org.Id} has ParentId {org.ParentId} and cannot be added to {Id}");
            }
            _childOrgs.Add(org);
        }
        public void AddUserMetrics(User user)
        {
            if (user.OrgId != Id)
            {
                throw new ArgumentException($"User {user.UserId} does not belong directly to Org {Id}");
            }
            _numUsers++;
            _numFiles += user.NumFiles;
        }

        public int ParentId { get; private set; }

        public int Id { get; private set; }

        public string Name { get; private set; }

        private SortedSet<Org> _childOrgs = new SortedSet<Org>(new ByOrgId());

        private int _numUsers = 0;

        private int _numFiles = 0;

        private class ByOrgId : IComparer<Org>
        {
            public int Compare(Org org1, Org org2)
            {
                return org1.Id.CompareTo(org2.Id);
            }
        }
    }
}
