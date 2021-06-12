using EngineerHomework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace EngineerHomework.Service
{
    public class DataLoadingUtil<T>
    {
		public static List<T> LoadData(string inputDataFilePath, IEntityBuilder<T> entityBuilder)
		{
			if (!IsInputFileValid(inputDataFilePath))
			{
				throw new FileNotFoundException();
			}

			var lines = File.ReadLines(inputDataFilePath);
			foreach (var line in lines)
			{
				entityBuilder.AddEntityFromCsvLine(line);
			}

			var entityList = entityBuilder.GetEntityList();
			Console.WriteLine($"Loaded data from file '{inputDataFilePath}'; Entity count: {entityList.Count}");
			return entityList;
		}

		private static bool IsInputFileValid(string filename)
		{
			bool success = File.Exists(filename);

			Console.WriteLine($"Validated input file '{filename}'; success: {success}");
			return success;
		}
	}
}
