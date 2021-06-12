using EngineerHomework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EngineerHomework.Tests
{
    [TestClass]
    public class OrgTests
    {
        [TestMethod]
        public void TestGenerateOrg()
        {
            var random = new Random();
            var expectedOrgFields = (
                OrgId: random.Next(1000),
                ParentOrgId: random.Next(1000),
                Name: Guid.NewGuid().ToString()
            );
            var csvLine = $"{expectedOrgFields.OrgId}, {expectedOrgFields.ParentOrgId}, {expectedOrgFields.Name}";

            var org = Org.Generate(csvLine);

            Assert.AreEqual(expectedOrgFields, (org.Id, org.ParentId, org.Name));
        }

        [DataTestMethod]
        [DataRow("2, null")]
        [DataRow("")]
        [DataRow("2, 1, Chimney, 45")]
        public void TestGenerateOrgThrowsFormatException(string invalidCsvLine)
        {
            var exc = Assert.ThrowsException<FormatException>(() => {
                Org.Generate(invalidCsvLine);
            });
            StringAssert.Contains(exc.Message, "Invalid line found in Org hierarchy");
        }

        [TestMethod]
        public void TestGenerateOrgHandlesNullParentId()
        {
            var org = Org.Generate("1, null, The Null One");
            Assert.AreEqual(org.ParentId, Org.ROOT_ORG_PARENT_ORG_ID);
        }

        [TestMethod]
        public void TestChildOrgs()
        {
            var rootOrg = Org.Generate("1, null, I am groot");
            var childOrgs = new List<Org>() {
                Org.Generate("2, 1, root.2"),
                Org.Generate("3, 1, root.3"),
                Org.Generate("6,1,root.6")
            };

            foreach (Org childOrg in childOrgs)
            {
                rootOrg.AddChildOrg(childOrg);
            }

            Assert.AreEqual(rootOrg.GetChildOrgs().Count, childOrgs.Count);
        }

        [DataTestMethod]
        [DataRow("3, null, cannot be root child")]
        [DataRow("3, 9, wrong parent id")]
        public void TestAddChildOrgWithWrongParentId(string childOrgCsvLine)
        {
            var parentOrg = Org.Generate("2, 1, Parent of the Year");
            var childOrg = Org.Generate(childOrgCsvLine);
            var exc = Assert.ThrowsException<ArgumentException>(() => {
                parentOrg.AddChildOrg(childOrg);
            });
            Assert.AreEqual(
                exc.Message, $"Org {childOrg.Id} has ParentId {childOrg.ParentId} and cannot be added to {parentOrg.Id}"
            );
        }

        [TestMethod]
        public void TestOrgUserMetrics()
        {
            var random = new Random();
            var rootOrg = Org.Generate("1, null, I am groot");
            var childOrgs = new List<(Org, List<int>)>() {
                (
                    Org.Generate("2, 1, root.2"),
                    new List<int> {
                        random.Next(1, 1000), random.Next(1, 99)
                    }
                ),
                (
                    Org.Generate("3, 1, root.3"),
                    new List<int> {
                        random.Next(50, 1000)
                    }
                ),
                (
                    Org.Generate("6, 1, root.6"),
                    new List<int> {}
                )
            };

            int expectedNumUsers = 0;
            int expectedNumFiles = 0;

            foreach ((Org childOrg, List<int> userFileCounts) in childOrgs)
            {
                rootOrg.AddChildOrg(childOrg);
                int expectedChildNumUsers = 0;
                int expectedChildNumFiles = 0;

                foreach (int userFileCount in userFileCounts)
                {
                    childOrg.AddUserMetrics(User.Generate($"{random.Next(1, 1000)}, {childOrg.Id}, {userFileCount}"));
                    expectedNumUsers++;
                    expectedNumFiles += userFileCount;
                    expectedChildNumUsers++;
                    expectedChildNumFiles += userFileCount;
                }

                Assert.AreEqual(
                    (childOrg.GetTotalNumUsers(), childOrg.GetTotalNumFiles()),
                    (expectedChildNumUsers, expectedChildNumFiles)
                );
            }

            var rootOrgUsers = random.Next(2, 6);
            for (int i = 0; i < rootOrgUsers; i++)
            {
                var numFilesForRootOrgUser = random.Next(42);
                rootOrg.AddUserMetrics(
                    User.Generate($"{random.Next(1000, 2000)}, {rootOrg.Id}, {numFilesForRootOrgUser}")
                );
                expectedNumFiles += numFilesForRootOrgUser;
            }
            expectedNumUsers += rootOrgUsers;

            Assert.AreEqual(
                (rootOrg.GetTotalNumUsers(), rootOrg.GetTotalNumFiles()),
                (expectedNumUsers, expectedNumFiles)
            );
        }

        [DataTestMethod]
        [DataRow("3, 8, 42")]
        [DataRow("4, 9, 42")]
        public void TestAddUserMetricsWithWrongOrgId(string userCsvLine)
        {
            var org = Org.Generate("1, null, The One");
            var user = User.Generate(userCsvLine);
            var exc = Assert.ThrowsException<ArgumentException>(() => {
                org.AddUserMetrics(user);
            });
            Assert.AreEqual(
                exc.Message, $"User {user.UserId} does not belong directly to Org {org.Id}"
            );
        }
    }
}
