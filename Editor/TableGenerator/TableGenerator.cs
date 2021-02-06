using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Unitils
{
	public static class TableGenerator
	{
		private class Configuration
		{
			public string filePath;
			public string folderName;
			public bool isWritable;
			public string separator;
			public string classNameEraser;
			public string classNameFormat;
		}


		public static void GenerateClass(TableGeneratorData data)
		{
			List<Configuration> configurations = GetConfigurations(data);

			configurations.ForEach(_ => GenerateClass(data, _));
			GenerateDBClass(data);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static void GenerateData(TableGeneratorData data)
		{
			List<Configuration> configurations = GetConfigurations(data);

			configurations.ForEach(_ => GenerateData(data, _));

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}


		private static List<Configuration> GetConfigurations(TableGeneratorData data)
		{
			if (data.Folders == null || data.Folders.Count <= 0) new List<Configuration>();

			List<Configuration> configurations = new List<Configuration>();

			foreach (TableGeneratorData.FolderData folderData in data.Folders) {
				if (!Directory.Exists(folderData.path) || !folderData.enabled) continue;

				if (!string.IsNullOrEmpty(folderData.classNameEraser)) {
					try {
						Regex regex = new Regex(folderData.classNameEraser);
					}
					catch {
						continue;
					}
				}
				if (!folderData.classNameFormat.Contains("*")) continue;

				string[] files = Directory.GetFiles(folderData.path, "*.csv");
				string outputPath = Path.Combine(data.ClassOutputFolder);

				foreach (string filePath in files) {
					configurations.Add(new Configuration
					{
						filePath = filePath,
						folderName = folderData.folderName,
						isWritable = folderData.isWritableTable,
						separator = folderData.separator,
						classNameEraser = folderData.classNameEraser,
						classNameFormat = folderData.classNameFormat
					});
				}
			}

			return configurations;
		}


		private static void GenerateClass(TableGeneratorData data, Configuration configuration)
		{
			if (!File.Exists(configuration.filePath)) return;
			CSVReader csvReader = new CSVReader();
			List<List<string>> csv = csvReader.ParseCSV(File.ReadAllText(configuration.filePath));

			string tableName = csv[0][0];
			List<string> columns = csv[1].Select(_ => _.ToLower()).ToList();
			List<string> keys = csv[2];
			List<string> types = csv[3];
			List<string> comments = csv[4];

			foreach (string variableType in types) {
				if (!IsValidateVariableType(variableType)) return;
			}

			int primaryKeyIndex = keys.FindIndex(_ => _.ToLower().Contains("primary"));
			if (primaryKeyIndex < 0) return;

			string spaceName = configuration.folderName.Replace('/', '.');

			#region Generate Class

			string className = tableName;
			if (!string.IsNullOrEmpty(configuration.separator)) {
				char separator = configuration.separator[0];
				string[] words = className.Split(separator);
				className = string.Join(separator.ToString(), words.Select(_ => Utils.Text.ToUpper(_, 0)));
			}
			if (!string.IsNullOrEmpty(configuration.classNameEraser)) {
				className = Regex.Replace(className, configuration.classNameEraser, "", RegexOptions.IgnoreCase);
			}
			className = configuration.classNameFormat.Replace("*", className);

			string outputFolderPath = Path.Combine(Application.dataPath, data.ClassOutputFolder, configuration.folderName);
			if (!Directory.Exists(outputFolderPath)) Directory.CreateDirectory(outputFolderPath);

			string usePropertyTemplate = configuration.isWritable ? WRITABLE_PROPERTY_TEMPLATE : READONLY_PROPERTY_TEMPLATE;

			string classBody = "";
			for (int i = 0; i < columns.Count; i++) {
				string[] words = columns[i].Split('_');
				string upperCamelName = string.Concat(words.Select(_ => Utils.Text.ToUpper(_, 0)));
				string lowerCamelName = Utils.Text.ToLower(upperCamelName, 0);

				classBody += usePropertyTemplate
					.Replace("[variable_type]", types[i])
					.Replace("[lower_camel_name]", lowerCamelName)
					.Replace("[upper_camel_name]", upperCamelName);
			}
			while (classBody.EndsWith("\n")) classBody = classBody.TrimEnd('\n');

			string classText = string.Format(CLASS_TEMPLATE, spaceName, className, classBody);
			File.WriteAllText(Path.Combine(outputFolderPath, $"{className}.cs"), classText);
			#endregion

			#region Generate Table Class

			string tableClassName = $"{className}Table";
			string inheritance = (configuration.isWritable ? "WritableTable" : "TableBase") + $"<{className}, {types[primaryKeyIndex]}>";

			classBody = "";

			classText = string.Format(TABLE_CLASS_TEMPLATE, spaceName, tableClassName, inheritance, classBody);
			File.WriteAllText(Path.Combine(outputFolderPath, $"{tableClassName}.cs"), classText);
			#endregion
		}

		private static void GenerateDBClass(TableGeneratorData data)
		{
		}


		private static void GenerateData(TableGeneratorData data, Configuration configuration)
		{
			CSVReader csvReader = new CSVReader();
			List<List<string>> csv = csvReader.ParseCSV(File.ReadAllText(configuration.filePath));

			string tableName = csv[0][0];
			List<string> columns = csv[1].Select(_ => _.ToLower()).ToList();

			string jsonText = "";

			for (int i = 5; i < csv.Count; i++) {
				string record = "{";
				for (int j = 0; j < columns.Count; j++) {
					record += $"\"{columns[j]}\":\"{csv[i][j].Replace("\"", "\\\"").Replace("\n", "\\n")}\",";
				}
				record = record.TrimEnd(',') + "}";
				jsonText += record;
			}
			jsonText = $"{{\"list\":[{jsonText}]}}";

			string outputFolderPath = Path.Combine(Application.dataPath, data.DataOutputFolder, configuration.folderName);
			if (!Directory.Exists(outputFolderPath)) Directory.CreateDirectory(outputFolderPath);

			File.WriteAllText(Path.Combine(outputFolderPath, $"{tableName}.json"), jsonText);
		}


		private static bool IsValidateVariableType(string variableType)
		{
			if (string.IsNullOrEmpty(variableType)) return false;
			return Regex.IsMatch("int|long|float|string", variableType);
		}


		private const string READONLY_PROPERTY_TEMPLATE =
			"\t\t[SerializeField] private [variable_type] [lower_camel_name];\n" +
			"\t\tpublic [variable_type] [upper_camel_name] => this.[lower_camel_name];\n\n";

		private const string WRITABLE_PROPERTY_TEMPLATE =
			"\t\t[SerializeField] private [variable_type] [lower_camel_name];\n" +
			"\t\tpublic [variable_type] [upper_camel_name] { get { return this.[lower_camel_name]; } set { this.[lower_camel_name] = value; } }\n\n";

		private const string CLASS_TEMPLATE =
			"using System;\n" +
			"using UnityEngine;\n" +
			"\n" +
			"namespace {0}\n" +
			"{{\n" +
			"\t[Serializable]\n" +
			"\tpublic partial class {1}\n" +
			"\t{{\n" +
			"{2}\n" +
			"\t}}\n" +
			"}}";

		private const string TABLE_CLASS_TEMPLATE =
			"using System;\n" +
			"using System.Collections.Generic;\n" +
			"using Unitils;\n" +
			"\n" +
			"namespace {0}\n" +
			"{{\n" +
			"\tpublic class {1} : {2}\n" +
			"\t{{\n" +
			"{3}\n" +
			"\t}}\n" +
			"}}";
	}
}