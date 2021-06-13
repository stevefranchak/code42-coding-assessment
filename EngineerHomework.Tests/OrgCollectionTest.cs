using EngineerHomework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace EngineerHomework.Tests
{
    [TestClass]
    public class OrgCollectionTests
    {
        private static List<string> s_testOrgCsvLines = new List<string> {
            "1, null, root1",
            "2, 1, root1.A",
            "3, 1, root1.B",
            "21, 2, root1.A.1",
            "22, 2, root1.A.2",
            "23, 22, root1.A.2.a",
            "31, 3, root1.B.1",
            "32, 3, root1.B.2",
            "321, 32, root1.B.2.a",
            "4, null, root2",
            "41, 4, root2.A",
            "42, 4, root2.B"
        };

        private static int s_rootOrgCount = 2;

        private static List<Org> GetTestOrgs()
        {
            return s_testOrgCsvLines.Select((csvString) => Org.Generate(csvString)).ToList();
        }

        [TestMethod]
        public void TestGenerateOrgCollection()
        {
            var orgs = GetTestOrgs();
            var orgCollection = OrgCollection.Generate(orgs);
            Assert.AreEqual(s_rootOrgCount, orgCollection.GetRootOrgIds().Count);

            // Sample a couple orgs from the OrgCollection
            var org = orgCollection.GetOrg(4);
            Assert.AreEqual(2, org.GetChildOrgs().Count);
            org = orgCollection.GetOrg(22);
            Assert.AreEqual(1, org.GetChildOrgs().Count);

            Assert.AreEqual(orgs.Count, orgCollection.Count);
        }

        [TestMethod]
        public void TestGenerateOrgCollectionOutOfOrder()
        {
            var orgs = GetTestOrgs();
            SwapOrgOrder(orgs, 1, 23);
            SwapOrgOrder(orgs, 2, 22);
            SwapOrgOrder(orgs, 4, 1);
            SwapOrgOrder(orgs, 21, 42);
            var orgCollection = OrgCollection.Generate(orgs);

            Assert.AreEqual(orgs.Count, orgCollection.Count);
            AssertOrgTreeOrder(orgCollection);
        }

        [TestMethod]
        public void TestOrgCollectionWithParentIdValueGreaterThanOrgIdValue()
        {
            var orgs = new List<Org> {
                Org.Generate("5, null, 5"),
                Org.Generate("2, 5, 5.2"),
                Org.Generate("1, 2, 5.2.3"),
            };
            var orgCollection = OrgCollection.Generate(orgs);

            Assert.AreEqual(orgs.Count, orgCollection.Count);
            CollectionAssert.AreEqual(new List<int> {5}, orgCollection.GetRootOrgIds());
            Assert.AreEqual(2, orgCollection.GetOrg(5).GetChildOrgs()[0].Id);
            Assert.AreEqual(1, orgCollection.GetOrg(2).GetChildOrgs()[0].Id);
            Assert.AreEqual(0, orgCollection.GetOrg(1).GetChildOrgs().Count);
        }

        [TestMethod]
        public void TestGenerateOrgCollectionDuplicateOrgIds()
        {
            var orgs = new List<Org> {
                Org.Generate("1, null, Root 1"),
                Org.Generate("1, null, Root 2"),
            };

            var orgCollection = OrgCollection.Generate(orgs);

            Assert.AreEqual("Root 1", orgCollection.GetOrg(1).Name);
        }

        [TestMethod]
        public void TestGetOrgTree()
        {
            AssertOrgTreeOrder(OrgCollection.Generate(GetTestOrgs()));
        }

        private static void AssertOrgTreeOrder(OrgCollection orgCollection)
        {
            var expectedOrgIdOrder = new List<int> {
                1, 2, 21, 22, 23, 3, 31, 32, 321, 4, 41, 42
            };

            // Using IEnumerable.SelectMany to flatten the list of lists
            List<Org> orgs = orgCollection.GetRootOrgIds()
                .Select(rootOrgId => orgCollection.GetOrgTree(rootOrgId, true))
                .SelectMany(orgTree => orgTree).ToList();

            for (int i = 0; i < expectedOrgIdOrder.Count; i++)
            {
                Assert.AreEqual(expectedOrgIdOrder[i], orgs[i].Id);
            }

            expectedOrgIdOrder.Remove(1);
            expectedOrgIdOrder.Remove(4);

            orgs = orgCollection.GetRootOrgIds()
                .Select(rootOrgId => orgCollection.GetOrgTree(rootOrgId, false))
                .SelectMany(orgTree => orgTree).ToList();

            for (int i = 0; i < expectedOrgIdOrder.Count; i++)
            {
                Assert.AreEqual(expectedOrgIdOrder[i], orgs[i].Id);
            }
        }

        // Assumption: used in test methods in which a provided orgId is known to exist
        private static void SwapOrgOrder(List<Org> orgs, int orgId1, int orgId2)
        {
            var (orgId1Index, orgId2Index) = (
                orgs.FindIndex((org) => org.Id == orgId1), orgs.FindIndex((org) => org.Id == orgId2)
            );
            (orgs[orgId1Index], orgs[orgId2Index]) = (orgs[orgId2Index], orgs[orgId1Index]);
        }
    }
}
