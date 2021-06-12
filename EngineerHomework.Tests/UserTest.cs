using EngineerHomework.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EngineerHomework.Tests
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void TestGenerateUser()
        {
            var random = new Random();
            var expectedUserFields = (
                UserId: random.Next(1000000),
                OrgId: random.Next(100),
                NumFiles: random.Next(50)
            );
            var csvLine = $"{expectedUserFields.UserId}, {expectedUserFields.OrgId}, {expectedUserFields.NumFiles}";

            var user = User.Generate(csvLine);

            Assert.AreEqual(expectedUserFields, (user.UserId, user.OrgId, user.NumFiles));
        }

        [DataTestMethod]
        [DataRow("2, 4")]
        [DataRow("")]
        [DataRow("4, 3, 44, 45")]
        public void TestGenerateUserThrowsFormatException(string invalidCsvLine)
        {
            var exc = Assert.ThrowsException<FormatException>(() => {
                User.Generate(invalidCsvLine);
            });
            StringAssert.Contains(exc.Message, "Invalid line found in User data");
        }
    }
}
