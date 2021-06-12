using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace EngineerHomework.Tests
{
    [TestClass]
    public class ProgramTest
    {
        private static string s_inputFileFolder = "Data";

        private static string s_orgHierarchyDataFile = "OrgHierarchyData.csv";

        private static string s_userDataFile = "UserData.csv";

        private static List<string> s_expectedFileContents = new List<string> {
            "1, 8, 188", "\t2, 4, 100", "\t\t21, 1, 40", "\t\t22, 1, 10", "\t\t\t23, 1, 10",
            "\t3, 3, 87", "\t\t31, 1, 1", "\t\t32, 1, 6", "\t\t\t321, 1, 6", "4, 2, 32",
            "\t41, 1, 20", "\t42, 0, 0"
        };

        [TestMethod]
        public void TestProgram()
        {
                string outputDirectory = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());
                string outputFilePath = Path.Join(outputDirectory, Program.OUTPUT_FILE_NAME);
                Directory.CreateDirectory(outputDirectory);

                try {
                    Program.Main(new string[] {
                        Path.Join(s_inputFileFolder, s_orgHierarchyDataFile),
                        Path.Join(s_inputFileFolder, s_userDataFile),
                        outputDirectory
                    });

                    Assert.IsTrue(File.Exists(outputFilePath));

                    using StreamReader reader = File.OpenText(outputFilePath);
                    for (int i = 0; i < s_expectedFileContents.Count; i++)
                    {
                        Assert.AreEqual(reader.ReadLine(), s_expectedFileContents[i]);
                    }
                }
                catch (Exception exc)
                {
                    Assert.Fail($"Caught unexpected exception {exc}: {exc.Message}");
                }
                finally
                {
                    Directory.Delete(outputDirectory, true);
                    Assert.IsFalse(File.Exists(outputFilePath));
                    Console.WriteLine("Deleted test output directory");
                }
        }
    }
}
