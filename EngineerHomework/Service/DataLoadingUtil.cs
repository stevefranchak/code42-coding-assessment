using EngineerHomework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EngineerHomework.Service
{
    public class DataLoadingUtil<T>
    {
        public static IEnumerable<T> LoadData(string inputDataFilePath, IEntityGenerator<T> entityGenerator)
        {
            if (!IsInputFileValid(inputDataFilePath))
            {
                throw new FileNotFoundException();
            }

            return File.ReadLines(inputDataFilePath).Select((csvLine) => entityGenerator.Generate(csvLine));
        }

        private static bool IsInputFileValid(string filename)
        {
            bool success = File.Exists(filename);

            Console.WriteLine($"Validated input file '{filename}'; success: {success}");
            return success;
        }
    }
}
