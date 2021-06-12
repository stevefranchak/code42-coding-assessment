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
            var outOfOrderParentIdToChildrenIdsMapping = new Dictionary<int, List<int>>();

            foreach (var org in orgs)
            {
                orgCollection._orgLookup.TryAdd(org.Id, org);

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
                        if (!outOfOrderParentIdToChildrenIdsMapping.ContainsKey(org.ParentId))
                        {
                            outOfOrderParentIdToChildrenIdsMapping[org.ParentId] = new List<int>();
                        }
                        outOfOrderParentIdToChildrenIdsMapping[org.ParentId].Add(org.Id);
                    }
                }

                // Check whether `org` itself is an out-of-order parent
                if (outOfOrderParentIdToChildrenIdsMapping.ContainsKey(org.Id))
                {
                    foreach (int childOrgId in outOfOrderParentIdToChildrenIdsMapping[org.Id])
                    {
                        org.AddChildOrg(orgCollection.GetOrg(childOrgId));
                    }
                    outOfOrderParentIdToChildrenIdsMapping.Remove(org.Id);
                }
            }

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