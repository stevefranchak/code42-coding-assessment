using EngineerHomework.Models;
using EngineerHomework.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EngineeringHomework.Tests
{
    [TestClass]
    public class DataLoadingUtilTest
    {
        private static string s_inputFileFolder = "Data";

        private static string s_orgHierarchyDataFile = "OrgHierarchyData.csv";

        private static string s_userDataFile = "UserData.csv";

        private static int s_rootOrgCount = 2;

        private static int s_orgCount = 12;

        private static int s_userCount = 10;

        private static int s_fileCount = 220;

        [TestMethod]
        public void TestLoadData_Org()
        {
            var inputFilePath = Path.Combine(s_inputFileFolder, s_orgHierarchyDataFile);
            List<Org> orgList = DataLoadingUtil<Org>.LoadData(inputFilePath, new OrgEntityBuilder());
            Assert.IsNotNull(orgList);
            Assert.AreEqual(s_orgCount, orgList.Count);

            // Make sure the total number of root orgs matches the expected
            var rootCount = orgList.Where(o => o.ParentId == Org.ROOT_ORG_PARENT_ORG_ID).Count();
            Assert.AreEqual(s_rootOrgCount, rootCount);
        }

        [TestMethod]
        public void TestLoadData_User()
        {
            var inputFilePath = Path.Combine(s_inputFileFolder, s_userDataFile);
            List<User> userList = DataLoadingUtil<User>.LoadData(inputFilePath, new UserEntityBuilder());
            Assert.IsNotNull(userList);
            Assert.AreEqual(s_userCount, userList.Count);

            // Make sure the total number of files matches the expected.
            int actualFileCount = userList.Sum(o => o.NumFiles);
            Assert.AreEqual(s_fileCount, actualFileCount);
        }
    }
}
