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
		private static string InputFileFolder = "Data";
		private static string OrgHierarchyDataFile = "OrgHierarchyData.csv";
		private static string UserDataFile = "UserData.csv";

		private static int RootOrgCount = 2;
		private static int OrgCount = 12;
		private static int UserCount = 10;
		private static int FileCount = 220;

		[TestMethod]
		public void TestLoadData_Org()
		{
			var inputFilePath = Path.Combine(InputFileFolder, OrgHierarchyDataFile);
			List<Org> orgList = DataLoadingUtil<Org>.LoadData(inputFilePath, new OrgEntityBuilder());
			Assert.IsNotNull(orgList);
			Assert.AreEqual(OrgCount, orgList.Count);

			// Make sure there is exactly one root Org
			var rootCount = orgList.Where(o => o.ParentId == Org.ROOT_ORG_PARENT_ORG_ID).Count();
			Assert.AreEqual(RootOrgCount, rootCount);
		}

		[TestMethod]
		public void testLoadData_User()
		{
			var inputFilePath = Path.Combine(InputFileFolder, UserDataFile);
			List<User> userList = DataLoadingUtil<User>.LoadData(inputFilePath, new UserEntityBuilder());
			Assert.IsNotNull(userList);
			Assert.AreEqual(UserCount, userList.Count);

			// Make sure the total number of files matches the expected.
			int actualFileCount = userList.Sum(o => o.NumFiles);
			Assert.AreEqual(FileCount, actualFileCount);
		}
	}
}
