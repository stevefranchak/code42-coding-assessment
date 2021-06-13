using EngineerHomework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EngineerHomework.Models
{
    /// <summary>
    /// A collection that groups multiple organization tree hierarchies.
    /// </summary>
    public class OrgCollection : IOrgCollection
    {
        /// <summary>
        /// Generates a new <see cref="OrgCollection"/> based on the provided <paramref name="orgs"/>.
        /// </summary>
        ///
        /// <remarks>
        /// <para>If any of the organizations in <paramref name="orgs"/> has a duplicate Id, the duplicate organizations
        /// are discarded and a warning line is printed to stdout for each duplicate organization.</para>
        /// <para>Organizations can be added out-of-order, meaning that an Org can be inserted before its Parent Org.
        /// Regardless of insertion order, the root node Ids and child node Ids are stored sorted by Org Id in ascending
        /// order, providing traversal parity.</para>
        /// </remarks>
        ///
        /// <param name="orgs">Enumerable of <see cref="Org"/>s to add to a new <see cref="OrgCollection"/>.</param>
        ///
        /// <returns>The generated <see cref="OrgCollection"/></returns>
        public static OrgCollection Generate(IEnumerable<Org> orgs)
        {
            var orgCollection = new OrgCollection();
            // Temporary cache of ParentId => List<ChildId> for cases where a child is added before the parent was added
            var outOfOrderParentIdToChildrenIdsMapping = new Dictionary<int, List<int>>();

            foreach (var org in orgs)
            {
                // If an org with a duplicate Id comes in, the duplicate org will not be added
                var addedOrg = orgCollection._orgLookup.TryAdd(org.Id, org);
                if (!addedOrg)
                {
                    Console.WriteLine(
                        $"Org ({org.Id}, {org.ParentId}, {org.Name}) cannot be added to OrgCollection; Org {org.Id} already exists"
                    );
                }

                if (org.ParentId == Org.ROOT_ORG_PARENT_ORG_ID)
                {
                    orgCollection._rootOrgIds.Add(org.Id);
                }
                else
                {
                    try
                    {
                        orgCollection.GetOrg(org.ParentId).AddChildOrg(org);
                    }
                    catch (KeyNotFoundException)
                    {
                        // If parent org was not added yet, temporarily cache the child org's Id for when the parent org is added
                        if (!outOfOrderParentIdToChildrenIdsMapping.ContainsKey(org.ParentId))
                        {
                            outOfOrderParentIdToChildrenIdsMapping[org.ParentId] = new List<int>();
                        }
                        outOfOrderParentIdToChildrenIdsMapping[org.ParentId].Add(org.Id);
                    }
                }

                // Check whether `org` itself is an out-of-order parent and handle if so
                if (outOfOrderParentIdToChildrenIdsMapping.ContainsKey(org.Id))
                {
                    foreach (int childOrgId in outOfOrderParentIdToChildrenIdsMapping[org.Id])
                    {
                        org.AddChildOrg(orgCollection.GetOrg(childOrgId));
                    }
                    outOfOrderParentIdToChildrenIdsMapping.Remove(org.Id);
                }
            }

            // Potential improvement: warn user if outOfOrderParentIdToChildrenIdsMapping.Count > 0
            return orgCollection;
        }

        /// <summary>
        /// Gets an <see cref="Org"/> from this collection by <paramref name="orgId"/>.
        /// </summary>
        ///
        /// <param name="orgId">ID of the organization to retrieve.</param>
        ///
        /// <returns>The found <see cref="Org"/></returns>
        ///
        /// <exception cref="KeyNotFoundException">Throw when no matching organization is found.</exception>
        public Org GetOrg(int orgId)
        {
            return _orgLookup[orgId];
        }

        /// <summary>
        /// Generates a linear <see cref="List&lt;T&gt;"/> of <see cref="Org"/>s in recursive tree order.
        /// Recursive tree order visits a node, then its child nodes recursively, and then its sibling nodes,
        /// in which each sibling node will also recursively visit its child nodes before subsequent siblings.
        /// The generated list represents a subtree if <paramref name="inclusive"/> is <c>true</c>; else, the
        /// generated list may contain N subtrees, where N is the number of child organizations of the
        /// organization keyed by <paramref name="orgId"/>.
        /// </summary>
        ///
        /// <param name="orgId">ID of the organization to use as the root of the subtree.</param>
        /// <param name="inclusive">
        /// If true, the Org matching <paramref name="orgId"/> is added as the first <see cref="Org"/> in the returned list.
        /// </param>
        ///
        /// <returns>The generated list of <see cref="Org"/>s that were visited in recursive tree order.</returns>
        ///
        /// <exception cref="KeyNotFoundException">
        /// Throw when no matching organization for <paramref name="orgId"/> is found.
        /// </exception>
        public List<Org> GetOrgTree(int orgId, bool inclusive)
        {
            var orgTree = new List<Org>();

            VisitOrgsInRecursiveOrder(orgId, (org, level) => {
                if (level > 0 || inclusive)
                {
                    orgTree.Add(org);
                }
            });

            return orgTree;
        }

        /// <summary>
        /// Visits each node of an <see cref="Org"/> subtree whose root is the organization keyed by
        /// <paramref name="orgId"/> in recursive tree order, executing <paramref name="orgAction"/> against each visited
        /// organization node. Recursive tree order visits a node, then its child nodes recursively, and then its
        /// sibling nodes, in which each sibling node will also recursively visit its child nodes before subsequent
        /// siblings.
        /// </summary>
        ///
        /// <param name="orgId">ID of the organization to use as the root of the subtree.</param>
        /// <param name="orgAction">
        /// An <see cref="Action"/> that takes two parameters: a visited <see cref="Org"/> and the depth of that
        /// <see cref="Org"/> in the subtree as an <see cref="System.Int32"/>.
        /// </param>
        ///
        /// <exception cref="KeyNotFoundException">
        /// Throw when no matching organization for <paramref name="orgId"/> is found.
        /// </exception>
        public void VisitOrgsInRecursiveOrder(int orgId, Action<Org, int> orgAction)
        {
            VisitOrgsInRecursiveOrder(GetOrg(orgId), orgAction);
        }

        private void VisitOrgsInRecursiveOrder(Org org, Action<Org, int> orgAction, int level = 0)
        {
            orgAction(org, level);

            foreach (var childOrg in org.GetChildOrgs())
            {
                VisitOrgsInRecursiveOrder(childOrg, orgAction, level + 1);
            }
        }

        /// <summary>Gets the list of root organization IDs in this collection.</summary>
        ///
        /// <returns>List of root organization IDs in ascending order.</returns>
        public List<int> GetRootOrgIds()
        {
            return _rootOrgIds.ToList();
        }

        /// <summary>The number of organizations in this collection.</summary>
        public int Count { get => _orgLookup.Count; }

        private Dictionary<int, Org> _orgLookup = new Dictionary<int, Org>();

        private SortedSet<int> _rootOrgIds = new SortedSet<int>();
    }
}
