using EngineerHomework.Models;
using EngineerHomework.Service;
using System;
using System.IO;

namespace EngineerHomework
{
    public class Program
    {
        private static int NUM_ARGS_EXPECTED = 3;
        private static string OUTPUT_FILE_NAME = "org-collection-output.txt";

        /// <summary>
        /// Main program entry point
        /// Parameters:
        /// -Fully qualified path to the input test file OrgHierarchyData.csv
        /// -Fully qualified path to the input test file UserData.csv
        /// -Fully qualified path to the output folder
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length != NUM_ARGS_EXPECTED)
            {
                Console.WriteLine($"Wrong number of command arguments; expected: { NUM_ARGS_EXPECTED}; actual:  {args.Length}");
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

            var orgList = DataLoadingUtil<Org>.LoadData(orgHierarchyFilePath, new OrgEntityBuilder());
            var userList = DataLoadingUtil<User>.LoadData(userDataFilePath, new UserEntityBuilder());

            // TODO: Implement the specified functionality using the two linear lists above.

        }
    }
}
