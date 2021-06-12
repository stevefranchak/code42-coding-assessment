using EngineerHomework.Models;
using EngineerHomework.Service;
using System;
using System.IO;
using System.Linq;

namespace EngineerHomework
{
    public class Program
    {
        private static int NUM_ARGS_EXPECTED = 3;

        public static string OUTPUT_FILE_NAME = "org-collection-output.txt";

        /// <summary>
        /// This is the main program entry point for a CLI tool that produces a breakdown of number of users
        /// and number of user files for each organization in an organization hierarchy.
        /// <para>Required CLI arguments:</para>
        /// <list type="number">
        /// <item>Fully qualified path to the input test file OrgHierarchyData.csv</item>
        /// <item>Fully qualified path to the input test file UserData.csv</item>
        /// <item>Fully qualified path to the output folder</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// This CLI tool
        /// </remarks>
        public static void Main(string[] args)
        {
            if (args.Length != NUM_ARGS_EXPECTED)
            {
                Console.WriteLine($"Wrong number of command arguments; expected: {NUM_ARGS_EXPECTED}; actual:  {args.Length}");
                Console.WriteLine("Expected arguments: [Org Hierarchy file spec] [User Data file spec] [Output file folder]");
                Environment.Exit(1);
            }

            var orgHierarchyFilePath = args[0];
            var userDataFilePath = args[1];
            var outputFolderPath = args[2];

            if (!Directory.Exists(outputFolderPath))
            {
                throw new Exception($"Output folder '{outputFolderPath}' does not exist or is not a folder.");
            }

            using var outputFileWriter = new StreamWriter(Path.Join(outputFolderPath, OUTPUT_FILE_NAME));

            var orgList = DataLoadingUtil<Org>.LoadData(orgHierarchyFilePath, new OrgEntityBuilder());
            var userList = DataLoadingUtil<User>.LoadData(userDataFilePath, new UserEntityBuilder());

            var orgCollection = OrgCollection.Generate(orgList);
            foreach (User user in userList)
            {
                orgCollection.GetOrg(user.OrgId).AddUserMetrics(user);
            }

            foreach (int rootOrgId in orgCollection.GetRootOrgIds())
            {
                orgCollection.VisitOrgsInRecursiveOrder(rootOrgId, (org, level) => {
                    outputFileWriter.Write($"{String.Concat(Enumerable.Repeat("\t", level))}");
                    outputFileWriter.WriteLine($"{org.Id}, {org.GetTotalNumUsers()}, {org.GetTotalNumFiles()}");
                });
            }
            outputFileWriter.Flush();
        }
    }
}
