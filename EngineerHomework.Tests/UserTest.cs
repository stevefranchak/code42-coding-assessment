using EngineerHomework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EngineerHomework.Tests
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
		public void GenerateUser()
		{
            var random = new Random();
            var orgId = random.Next(100);
            var userId = random.Next(1000000);
            var numFiles = random.Next(50);

            var csvLine = string.Format($"{userId}, {orgId}, {numFiles}");
            var user = User.Generate(csvLine);

            Assert.AreEqual(orgId, user.OrgId);
            Assert.AreEqual(userId, user.UserId);
            Assert.AreEqual(numFiles, user.NumFiles);
        }
	}
}
