using EngineerHomework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EngineerHomework.Models
{
    public class OrgCollection : IOrgCollection
    {
        public static OrgCollection Generate(List<Org> orgs)
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

        public Org GetOrg(int orgId)
        {
            return _orgLookup[orgId];
        }

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

        public void VisitOrgsInRecursiveOrder(Org org, Action<Org, int> orgAction, int level = 0)
        {
            orgAction(org, level);

            foreach (var childOrg in org.GetChildOrgs())
            {
                VisitOrgsInRecursiveOrder(childOrg, orgAction, level + 1);
            }
        }

        public void VisitOrgsInRecursiveOrder(int orgId, Action<Org, int> orgAction)
        {
            VisitOrgsInRecursiveOrder(GetOrg(orgId), orgAction);
        }

        public List<int> GetRootOrgIds()
        {
            return _rootOrgIds.ToList();
        }

        public int Count { get => _orgLookup.Count; }

        private Dictionary<int, Org> _orgLookup = new Dictionary<int, Org>();

        private SortedSet<int> _rootOrgIds = new SortedSet<int>();
    }
}
