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

		private class SecondaryInfo
		{
			public bool use;
			public string type;
			public string selector;
			public string findMethod;
		}


		public static void GenerateClass(TableGeneratorData data)
		{
			List<Configuration> configurations = GetConfigurations(data);
			if (configurations.Count <= 0) return;

			configurations.ForEach(_ => GenerateClass(data, _));

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static void GenerateData(TableGeneratorData data)
		{
			List<Configuration> configurations = GetConfigurations(data);
			if (configurations.Count <= 0) return;

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
			List<string> columns = csv[1];
			List<string> keys = csv[2];
			List<string> types = csv[3];
			List<string> comments = csv[4];

			columns = columns.Select(_ => _.ToLower()).ToList();

			if (string.IsNullOrEmpty(tableName)) {
				Debug.Log($"[TableGenerator] table name not found. ({configuration.filePath})");
			}

			foreach (string variableType in types) {
				if (!IsValidateVariableType(variableType)) {
					Debug.Log($"[TableGenerator] table name: {tableName}, unsupported variable type. {variableType}");
					return;
				}
			}

			int primaryIndex = keys.FindIndex(_ => _.ToLower().Contains("primary"));
			if (primaryIndex < 0) {
				Debug.Log($"[TableGenerator] table name: {tableName}, primary key not found.");
			}
			
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

			string usePropertyFormat = configuration.isWritable ? WRITABLE_PROPERTY_TEMPLATE : READONLY_PROPERTY_TEMPLATE;

			string classBody = "";
			for (int i = 0; i < columns.Count; i++) {
				string[] words = columns[i].Split('_');
				string upperCamelName = string.Concat(words.Select(_ => Utils.Text.ToUpper(_, 0)));
				string lowerCamelName = Utils.Text.ToLower(upperCamelName, 0);
				string comment = comments[i].Replace("\n", "\n\t\t/// ");
				classBody += string.Format(usePropertyFormat, types[i], lowerCamelName, upperCamelName, comment);
			}
			while (classBody.EndsWith("\n")) classBody = classBody.TrimEnd('\n');

			string classText = string.Format(CLASS_TEMPLATE, spaceName, className, classBody);
			File.WriteAllText(Path.Combine(outputFolderPath, $"{className}.cs"), classText);
			#endregion

			#region Generate Table Class

			string primaryType = types[primaryIndex];
			string primaryUpperCamelName = string.Concat(columns[primaryIndex].Split('_').Select(_ => Utils.Text.ToUpper(_, 0)));
			string primaryLowerCamelName = Utils.Text.ToLower(primaryUpperCamelName, 0);

			string tableClassName = $"{className}Table";
			string inheritance = (configuration.isWritable ? "WritableTable" : "TableBase") + $"<{className}, {primaryType}>";

			SecondaryInfo secondaryInfo = GetSecondaryInfo(columns, keys, types);

			classBody = string.Format(PRIMARY_PROPERTY_TEMPLATE, className, primaryType, primaryUpperCamelName);

			if (secondaryInfo.use) {
				classBody += string.Format(SECONDARY_PROPERTY_TEMPLATE, className, secondaryInfo.type, className);
				classBody += string.Format(CONSTRUCTOR_TEMPLATE_FOR_SECONDARY, tableClassName, className, secondaryInfo.selector, secondaryInfo.type);
				classBody += string.Format(FIND_BY_METHOD_TEMPLATE, className, primaryUpperCamelName, primaryType, primaryLowerCamelName);
				classBody += string.Format(FIND_BY_SECONDARY_METHOD_TEMPLATE, className, secondaryInfo.findMethod, secondaryInfo.type);
			}
			else {
				classBody += string.Format(CONSTRUCTOR_TEMPLATE, tableClassName, className);
				classBody += string.Format(FIND_BY_METHOD_TEMPLATE, className, primaryUpperCamelName, primaryType, primaryLowerCamelName);
			}

			while (classBody.EndsWith("\n")) classBody = classBody.TrimEnd('\n');

			classText = string.Format(TABLE_CLASS_TEMPLATE, spaceName, tableClassName, inheritance, classBody);
			File.WriteAllText(Path.Combine(outputFolderPath, $"{tableClassName}.cs"), classText);
			#endregion
		}


		private static bool IsValidateVariableType(string variableType)
		{
			if (string.IsNullOrEmpty(variableType)) return false;
			return Regex.IsMatch("int|long|float|string", variableType);
		}

		private static SecondaryInfo GetSecondaryInfo(List<string> columns, List<string> keys, List<string> types)
		{
			List<int> secondaryIndices = new List<int>();
			for (int i = 0; i < keys.Count; i++) {
				if (keys[i].ToLower().Contains("secondary")) secondaryIndices.Add(i);
			}

			List<(string type, string upperCamelName, string lowerCamelName)> secondaryProperties = secondaryIndices
				.Select(_ =>
				{
					string upperCamel = string.Concat(columns[_].Split('_').Select(word => Utils.Text.ToUpper(word, 0)));
					return (types[_], upperCamel, Utils.Text.ToLower(upperCamel, 0));
				})
				.ToList();

			bool use = secondaryProperties.Count > 0;
			string type = "";
			string selector = "";
			string findMethod = "";

			if (use) {
				if (secondaryProperties.Count == 1) {
					type = secondaryProperties[0].type;
					selector = $"_.{secondaryProperties[0].upperCamelName}";
					findMethod = secondaryProperties[0].upperCamelName;
				}
				else {
					type = "(";
					selector = "(";

					for (int i = 0; i < secondaryProperties.Count; i++) {
						type += $"{secondaryProperties[i].type} {secondaryProperties[i].lowerCamelName}, ";
						selector += $"_.{secondaryProperties[i].upperCamelName}, ";
						findMethod += $"{secondaryProperties[i].upperCamelName}And";
					}
					type = type.Substring(0, type.Length - 2) + ")";
					selector = selector.Substring(0, selector.Length - 2) + ")";
					findMethod = findMethod.Substring(0, findMethod.Length - 3);
				}
			}

			return new SecondaryInfo { use = use, type = type, selector = selector, findMethod = findMethod };
		}


		private static void GenerateData(TableGeneratorData data, Configuration configuration)
		{
			CSVReader csvReader = new CSVReader();
			List<List<string>> csv = csvReader.ParseCSV(File.ReadAllText(configuration.filePath));

			string tableName = csv[0][0];
			List<string> columns = csv[1].Select(_ => _.ToLower()).ToList();

			for (int i = 0; i < columns.Count; i++) {
				columns[i] = string.Concat(columns[i].Split('_').Select(word => Utils.Text.ToUpper(word, 0)));
				columns[i] = Utils.Text.ToLower(columns[i], 0);
			}

			string jsonText = "";

			for (int i = 5; i < csv.Count; i++) {
				string record = "{";
				for (int j = 0; j < columns.Count; j++) {
					record += $"\"{columns[j]}\":\"{csv[i][j].Replace("\"", "\\\"").Replace("\n", "\\n")}\",";
				}
				record = record.TrimEnd(',') + "},";
				jsonText += record;
			}
			jsonText = $"{{\"list\":[{jsonText.TrimEnd(',')}]}}";

			string outputFolderPath = Path.Combine(Application.dataPath, data.DataOutputFolder, configuration.folderName);
			if (!Directory.Exists(outputFolderPath)) Directory.CreateDirectory(outputFolderPath);

			File.WriteAllText(Path.Combine(outputFolderPath, $"{tableName}.json"), jsonText);
		}



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

		private const string READONLY_PROPERTY_TEMPLATE =
			"\t\t/// <summary>\n" +
			"\t\t/// {3}\n" +
			"\t\t/// </summary>\n" +
			"\t\t[SerializeField] private {0} {1};\n" +
			"\t\tpublic {0} {2} => this.{1};\n\n";

		private const string WRITABLE_PROPERTY_TEMPLATE =
			"\t\t/// <summary>\n" +
			"\t\t/// {3}\n" +
			"\t\t/// </summary>\n" +
			"\t\t[SerializeField] private {0} {1};\n" +
			"\t\tpublic {0} {2} {{ get {{ return this.{1}; }} set {{ this.{1} = value; }} }}\n\n";


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

		private const string PRIMARY_PROPERTY_TEMPLATE =
			"\t\tprotected override Func<{0}, {1}> PrimaryKeySelector => _ => _.{2};\n\n";

		private const string SECONDARY_PROPERTY_TEMPLATE =
			"\t\tprivate readonly Func<{0}, {1}> secondaryIndexSelector;\n" +
			"\t\tprivate readonly {2}[] secondaryIndex;\n\n";

		private const string CONSTRUCTOR_TEMPLATE =
			"\t\tpublic {0}({1}[] source) : base(source) {{}}\n\n";

		private const string CONSTRUCTOR_TEMPLATE_FOR_SECONDARY =
			"\t\tpublic {0}({1}[] source) : base(source)\n" +
			"\t\t{{\n" +
			"\t\t\tthis.secondaryIndexSelector = _ => {2};\n" +
			"\t\t\tthis.secondaryIndex = this.CloneAndSortBy(this.secondaryIndexSelector, Comparer<{3}>.Default);\n" +
			"\t\t}}\n\n";

		private const string FIND_BY_METHOD_TEMPLATE =
			"\t\tpublic {0} FindBy{1}({2} {3})\n" +
			"\t\t{{\n" +
			"\t\t\treturn this.FindBy(this.PrimaryKeySelector, Comparer<{2}>.Default, {3});\n" +
			"\t\t}}\n\n";

		private const string FIND_BY_SECONDARY_METHOD_TEMPLATE =
			"\t\tpublic RangeList<{0}> FindBy{1}({2} key)\n" +
			"\t\t{{\n" +
			"\t\t\treturn this.FindMany(this.secondaryIndex, this.secondaryIndexSelector, Comparer<{2}>.Default, key);\n" +
			"\t\t}}\n\n";
	}
}