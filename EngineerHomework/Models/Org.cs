using System;
using System.Collections.Generic;
using System.Linq;

namespace EngineerHomework.Models
{
    public class Org
    {
        /// <summary>
        /// When used as an organization's <see cref="ParentId"/>, indicates that the organization is the root node of an
        /// organization hierarchy tree.
        /// </summary>
        public const int ROOT_ORG_PARENT_ORG_ID = 0;

        private const int ORG_DATA_TOKENS_PER_LINE = 3;

        /// <summary>
        /// Generates a new <see cref="Org"/> instance with fields set to the column values of the provided
        /// <paramref name="csvLine"/>.
        /// </summary>
        ///
        /// <param name="csvLine">String in the format "orgId, parentOrgId, name"</param>
        ///
        /// <returns>The generated <see cref="Org"/></returns>
        ///
        /// <exception cref="FormatException">Throw when there are not exactly three column values</exception>
        /// <exception cref="FormatException">Throw when a non-int32 value is provided for the first column value</exception>
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

        private Org(int parentId, int id, string name)
        {
            ParentId = parentId;
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Gets the total number of users that directly report to this organization plus any of this organization's
        /// child organizations.
        /// </summary>
        ///
        /// <returns>Total number of users in this organization plus its child organizations, recursively.</returns>
        public int GetTotalNumUsers()
        {
            return _numUsers + (
                from childOrg in GetChildOrgs()
                select childOrg.GetTotalNumUsers()
            ).Sum();
        }

        /// <summary>
        /// Gets the total number of files belonging to users that directly report to this organization plus the user
        /// files of this organization's child organizations.
        /// </summary>
        ///
        /// <returns>Total number of user files in this organization plus its child organizations, recursively.</returns>
        public int GetTotalNumFiles()
        {
            return _numFiles + (
                from childOrg in GetChildOrgs()
                select childOrg.GetTotalNumFiles()
            ).Sum();
        }

        /// <summary>
        /// Gets the child organizations of this organization.
        /// </summary>
        ///
        /// <returns>List of child organizations sorted by <c>Id</c> in ascending order.</returns>
        public List<Org> GetChildOrgs()
        {
            return _childOrgs.ToList();
        }

        /// <summary>
        /// Adds a child organization to this organization.
        /// </summary>
        ///
        /// <param name="org">Organization to add to this organization.</param>
        ///
        /// <exception cref="ArgumentException">
        /// Throw when the provided organization's ParentId does not equal this organization's Id.
        /// </exception>
        public void AddChildOrg(Org org)
        {
            if (org.ParentId != Id)
            {
                throw new ArgumentException($"Org {org.Id} has ParentId {org.ParentId} and cannot be added to {Id}");
            }
            _childOrgs.Add(org);
        }

        /// <summary>
        /// Adds user metrics for a <see cref="User"/> that reports directly to this organization.
        /// </summary>
        ///
        /// <remarks>
        /// There are no measures in place to prevent the same <see cref="User"/>'s metrics from being added
        /// more than once. This method does not add the <see cref="User"/> instance to a collection field in this
        /// <see cref="Org"/> instance since there is no business requirement to get specific <see cref="User"/> instances
        /// of an organization.
        /// </remarks>
        ///
        /// <param name="user">User whose metrics are to be added to this organization.</param>
        ///
        /// <exception cref="ArgumentException">
        /// Throw when the provided user's OrgId does not equal this organization's Id.
        /// </exception>
        public void AddUserMetrics(User user)
        {
            if (user.OrgId != Id)
            {
                throw new ArgumentException($"User {user.UserId} does not belong directly to Org {Id}");
            }
            _numUsers++;
            _numFiles += user.NumFiles;
        }

        /// <summary>This organization's ID</summary>
        public int Id { get; private set; }

        /// <summary>This organization's parent organization's ID</summary>
        public int ParentId { get; private set; }

        /// <summary>This organization's name</summary>
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
