using EngineerHomework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EngineerHomework.Tests
{
    [TestClass]
    public class OrgTests
    {
        [TestMethod]
		public void GenerateOrg()
		{
			var random = new Random();
			var parentOrgId = random.Next(1000);
			var orgId = random.Next(1000);
			var name = Guid.NewGuid().ToString();

			var csvLine = string.Format($"{orgId}, {parentOrgId}, {name}");
			var org = Org.Generate(csvLine);

			Assert.AreEqual(parentOrgId, org.ParentId);
			Assert.AreEqual(orgId, org.Id);
			Assert.AreEqual(name, org.Name);
		}
	}
}
