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
		public static void GenerateClass(TableGeneratorData data)
		{
			List<Util.Configuration> configurations = Util.GetConfigurations(data);
			if (configurations.Count <= 0) return;

			List<Util.DBInfo> dbInfos = new List<Util.DBInfo>();
			configurations.ForEach(_ => GenerateClass(data, _, ref dbInfos));

			GenerateDB(dbInfos);

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static void GenerateData(TableGeneratorData data)
		{
			List<Util.Configuration> configurations = Util.GetConfigurations(data);
			if (configurations.Count <= 0) return;

			configurations.ForEach(_ => GenerateData(data, _));

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}


		private static void GenerateClass(TableGeneratorData data, Util.Configuration configuration, ref List<Util.DBInfo> dbInfos)
		{
			if (!File.Exists(configuration.filePath)) return;
			CSVReader csvReader = new CSVReader();
			List<List<string>> csv = csvReader.ParseCSV(File.ReadAllText(configuration.filePath));

			Util.CsvHeader csvHeader = new Util.CsvHeader
			{
				tableName = csv[0][0],
				columns = csv[1].Select(_ => _.ToLower()).ToList(),
				attributes = csv[2],
				types = csv[3],
				comments = csv[4],
				primaryIndex = csv[2].FindIndex(_ => _.ToLower().Contains("primary"))
			};

			if (string.IsNullOrEmpty(csvHeader.tableName)) {
				Debug.Log($"[TableGenerator] table name not found. ({configuration.filePath})");
				return;
			}
			if (csvHeader.primaryIndex < 0) {
				Debug.Log($"[TableGenerator] table name: {csvHeader.tableName}, primary key not found.");
				return;
			}

			foreach (string variableType in csvHeader.types) {
				if (!Util.IsValidateVariableType(variableType)) {
					Debug.Log($"[TableGenerator] table name: {csvHeader.tableName}, unsupported variable type. {variableType}");
					return;
				}
			}

			string spaceName = configuration.folderName.Replace("/", ".");
			string modelClassName = Util.GetModelClassName(configuration, csvHeader);
			string tableClassName = $"{modelClassName}Table";

			string outputFolderPath = Path.Combine(Application.dataPath, data.ClassOutputFolder, configuration.folderName);
			if (!Directory.Exists(outputFolderPath)) Directory.CreateDirectory(outputFolderPath);

			GenerateModelClass(configuration, csvHeader, outputFolderPath, spaceName, modelClassName);
			if (configuration.isWritable) {
				GenerateWritableTableClass(csvHeader, outputFolderPath, spaceName, modelClassName, tableClassName);
			}
			else {
				GenerateReadonlyTableClass(csvHeader, outputFolderPath, spaceName, modelClassName, tableClassName);
			}

			Util.DBInfo dbInfo = dbInfos.FirstOrDefault(_ => _.spaceName == spaceName);
			if (dbInfo == null) {
				dbInfo = new Util.DBInfo { spaceName = spaceName, outputPath = outputFolderPath };
				dbInfo.tables.Add(tableClassName);
				dbInfos.Add(dbInfo);
			}
			else {
				dbInfo.tables.Add(tableClassName);
			}
		}

		private static void GenerateModelClass(Util.Configuration configuration, Util.CsvHeader csvHeader, string outputFolderPath, string spaceName, string modelClassName)
		{
			string usePropertyTemplate = configuration.isWritable
				? TableGeneratorTemplate.Model.WRITABLE_PROPERTY
				: TableGeneratorTemplate.Model.READONLY_PROPERTY;

			string classBody = "";
			for (int i = 0; i < csvHeader.columns.Count; i++) {
				string[] words = csvHeader.columns[i].Split('_');
				string upperCamelName = string.Concat(words.Select(_ => Utils.Text.ToUpper(_, 0)));
				string lowerCamelName = Utils.Text.ToLower(upperCamelName, 0);
				string comment = csvHeader.comments[i].Replace("\n", "\n\t\t/// ");
				classBody += string.Format(usePropertyTemplate, csvHeader.types[i], lowerCamelName, upperCamelName, comment);
			}
			while (classBody.EndsWith("\n")) classBody = classBody.TrimEnd('\n');

			string classText = string.Format(TableGeneratorTemplate.Model.CLASS, spaceName, modelClassName, classBody);
			File.WriteAllText(Path.Combine(outputFolderPath, $"{modelClassName}.cs"), classText);
		}

		private static void GenerateReadonlyTableClass(Util.CsvHeader csvHeader, string outputFolderPath, string spaceName, string modelClassName, string tableClassName)
		{
			string primaryType = csvHeader.types[csvHeader.primaryIndex];
			string primaryUpperCamelName = string.Concat(csvHeader.columns[csvHeader.primaryIndex].Split('_').Select(_ => Utils.Text.ToUpper(_, 0)));
			string primaryLowerCamelName = Utils.Text.ToLower(primaryUpperCamelName, 0);

			Util.SecondaryInfo secondaryInfo = Util.GetSecondaryInfo(csvHeader);

			string classBody = string.Format(TableGeneratorTemplate.RTable.PRIMARY_PROPERTY, modelClassName, primaryType);

			if (secondaryInfo.use) {
				string useFindBySecondaryMethodTemplate = secondaryInfo.isUnique
					? TableGeneratorTemplate.RTable.FIND_BY_SECONDARY_METHOD
					: TableGeneratorTemplate.RTable.FIND_BY_SECONDARY_NONUNIQUE_METHOD;

				classBody += string.Format(TableGeneratorTemplate.RTable.SECONDARY_PROPERTY, modelClassName, secondaryInfo.type, modelClassName);
				classBody += string.Format(TableGeneratorTemplate.RTable.CONSTRUCTOR_ALSO_SECONDARY, tableClassName, modelClassName, primaryUpperCamelName, primaryType, secondaryInfo.selector, secondaryInfo.type);
				classBody += string.Format(TableGeneratorTemplate.RTable.FIND_BY_METHOD, modelClassName, primaryUpperCamelName, primaryType, primaryLowerCamelName);
				classBody += string.Format(useFindBySecondaryMethodTemplate, modelClassName, secondaryInfo.findMethod, secondaryInfo.type);
			}
			else {
				classBody += string.Format(TableGeneratorTemplate.RTable.CONSTRUCTOR, tableClassName, modelClassName, primaryUpperCamelName, primaryType);
				classBody += string.Format(TableGeneratorTemplate.RTable.FIND_BY_METHOD, modelClassName, primaryUpperCamelName, primaryType, primaryLowerCamelName);
			}

			while (classBody.EndsWith("\n")) classBody = classBody.TrimEnd('\n');

			string classText = string.Format(TableGeneratorTemplate.RTable.CLASS, spaceName, tableClassName, modelClassName, classBody);
			File.WriteAllText(Path.Combine(outputFolderPath, $"{tableClassName}.cs"), classText);
		}

		private static void GenerateWritableTableClass(Util.CsvHeader csvHeader, string outputFolderPath, string spaceName, string modelClassName, string tableClassName)
		{
			string primaryType = csvHeader.types[csvHeader.primaryIndex];
			string primaryUpperCamelName = string.Concat(csvHeader.columns[csvHeader.primaryIndex].Split('_').Select(_ => Utils.Text.ToUpper(_, 0)));
			string primaryLowerCamelName = Utils.Text.ToLower(primaryUpperCamelName, 0);

			Util.SecondaryInfo secondaryInfo = Util.GetSecondaryInfo(csvHeader);

			string classBody = string.Format(TableGeneratorTemplate.WTable.PRIMARY_PROPERTY, modelClassName, primaryType);

			if (secondaryInfo.use) {
				string useFindBySecondaryMethodTemplate = secondaryInfo.isUnique
					? TableGeneratorTemplate.WTable.FIND_BY_SECONDARY_METHOD
					: TableGeneratorTemplate.WTable.FIND_BY_SECONDARY_NONUNIQUE_METHOD;

				classBody += string.Format(TableGeneratorTemplate.WTable.SECONDARY_PROPERTY, modelClassName, secondaryInfo.type, modelClassName);
				classBody += string.Format(TableGeneratorTemplate.WTable.CONSTRUCTOR_ALSO_SECONDARY, tableClassName, modelClassName, primaryUpperCamelName, primaryType, secondaryInfo.selector, secondaryInfo.type);
				classBody += string.Format(TableGeneratorTemplate.WTable.FIND_BY_METHOD, modelClassName, primaryUpperCamelName, primaryType, primaryLowerCamelName);
				classBody += string.Format(useFindBySecondaryMethodTemplate, modelClassName, secondaryInfo.findMethod, secondaryInfo.type);
				classBody += string.Format(TableGeneratorTemplate.WTable.ADD_METHOD_ALSO_SECONDARY, modelClassName, primaryType, primaryUpperCamelName, secondaryInfo.type);
				classBody += string.Format(TableGeneratorTemplate.WTable.REMOVE_KEY_METHOD, primaryLowerCamelName, primaryType);
				classBody += string.Format(TableGeneratorTemplate.WTable.REMOVE_ITEM_METHOD_ALSO_SECONDARY, modelClassName);
			}
			else {
				classBody += string.Format(TableGeneratorTemplate.WTable.CONSTRUCTOR, tableClassName, modelClassName, primaryUpperCamelName, primaryType);
				classBody += string.Format(TableGeneratorTemplate.WTable.FIND_BY_METHOD, modelClassName, primaryUpperCamelName, primaryType, primaryLowerCamelName);
				classBody += string.Format(TableGeneratorTemplate.WTable.ADD_METHOD, modelClassName, primaryType, primaryUpperCamelName);
				classBody += string.Format(TableGeneratorTemplate.WTable.REMOVE_KEY_METHOD, primaryLowerCamelName, primaryType);
				classBody += string.Format(TableGeneratorTemplate.WTable.REMOVE_ITEM_METHOD, modelClassName);
			}

			while (classBody.EndsWith("\n")) classBody = classBody.TrimEnd('\n');

			string classText = string.Format(TableGeneratorTemplate.WTable.CLASS, spaceName, tableClassName, modelClassName, classBody);
			File.WriteAllText(Path.Combine(outputFolderPath, $"{tableClassName}.cs"), classText);
		}


		private static void GenerateDB(List<Util.DBInfo> dbConfigurations)
		{
			foreach (Util.DBInfo dbInfo in dbConfigurations) {
				string spaceName = dbInfo.spaceName;
				string dbClassName = $"{spaceName.Replace(".", "")}DB";
				string properties = "";

				foreach (string table in dbInfo.tables) {
					properties += string.Format(TableGeneratorTemplate.DB.PROPERTY, table);
				}

				string classText = string.Format(TableGeneratorTemplate.DB.CLASS, spaceName, dbClassName, properties);
				File.WriteAllText(Path.Combine(dbInfo.outputPath, $"{dbClassName}.cs"), classText);
			}
		}


		private static void GenerateData(TableGeneratorData data, Util.Configuration configuration)
		{
			CSVReader csvReader = new CSVReader();
			List<List<string>> csv = csvReader.ParseCSV(File.ReadAllText(configuration.filePath));

			string tableName = csv[0][0];
			
			int primaryIndex = csv[2].FindIndex(_ => _.ToLower().Contains("primary"));
			if (primaryIndex < 0) {
				Debug.Log($"[TableGenerator] table name: {tableName}, primary key not found.");
				return;
			}
			string primaryType = csv[3][primaryIndex];

			List<string> columns = csv[1].Select(_ => _.ToLower()).ToList();
			for (int i = 0; i < columns.Count; i++) {
				columns[i] = string.Concat(columns[i].Split('_').Select(word => Utils.Text.ToUpper(word, 0)));
				columns[i] = Utils.Text.ToLower(columns[i], 0);
			}

			csv.RemoveRange(0, 5);

			switch (primaryType) {
				case "int": 
					Comparer<int> intComparer = Comparer<int>.Default;
					csv.Sort((a, b) => intComparer.Compare(int.Parse(a[primaryIndex]), int.Parse(b[primaryIndex])));
					break;
				case "long":
					Comparer<long> longLomparer = Comparer<long>.Default;
					csv.Sort((a, b) => longLomparer.Compare(long.Parse(a[primaryIndex]), long.Parse(b[primaryIndex])));
					break;
				case "float":
					Comparer<float> floatComparer = Comparer<float>.Default;
					csv.Sort((a, b) => floatComparer.Compare(float.Parse(a[primaryIndex]), float.Parse(b[primaryIndex])));
					break;
				case "string":
					Comparer<string> stringComparer = Comparer<string>.Default;
					csv.Sort((a, b) => stringComparer.Compare(a[primaryIndex], b[primaryIndex]));
					break;
				default:
					Debug.Log($"[TableGenerator] table name: {tableName}, unsupported primary variable type. {primaryType}");
					break;
			}

			string jsonText = "";

			for (int i = 0; i < csv.Count; i++) {
				string record = "{";
				for (int j = 0; j < columns.Count; j++) {
					record += $"\"{columns[j]}\":\"{csv[i][j].Replace("\"", "\\\"").Replace("\n", "\\n")}\",";
				}
				record = record.TrimEnd(',') + "},";
				jsonText += record;
			}
			jsonText = $"{{\"list\":[{jsonText.TrimEnd(',')}]}}";

			string outputFolderPath = ""; Path.Combine(Application.dataPath, data.DataOutputFolder, configuration.folderName);
			string fileName = "";

			if (data.IsDataFileNameToMD5) {
				outputFolderPath = Path.Combine(Application.dataPath, data.DataOutputFolder);
				fileName = Utils.Security.GetMD5(Path.Combine(configuration.folderName, $"{tableName}.json"));
			}
			else {
				outputFolderPath = Path.Combine(Application.dataPath, data.DataOutputFolder, configuration.folderName);
				fileName = Path.Combine(configuration.folderName, $"{tableName}.json");
			}

			if (!Directory.Exists(outputFolderPath)) Directory.CreateDirectory(outputFolderPath);

			string outputFilePath = Path.Combine(Application.dataPath, data.DataOutputFolder, fileName);

			if (data.EncryptionType == Define.EncryptionType.None) {
				File.WriteAllText(outputFilePath, jsonText);
				return;
			}
			if (data.EncryptionType == Define.EncryptionType.AES) {
				byte[] bytes = Utils.Security.EncryptAES(jsonText, data.EncryptAesKey, data.EncryptAesIv);
				File.WriteAllBytes(outputFilePath, bytes);
			}
		}



		private static class Util
		{
			public class Configuration
			{
				public string filePath;
				public string folderName;
				public bool isWritable;
				public string classNameSeparator;
				public string classNameEraser;
				public string classNameFormat;
			}

			public class DBInfo
			{
				public string spaceName;
				public string outputPath;
				public List<string> tables = new List<string>();
			}

			public class CsvHeader
			{
				public string tableName;
				public List<string> columns;
				public List<string> attributes;
				public List<string> types;
				public List<string> comments;
				public int primaryIndex;
			}

			public class SecondaryInfo
			{
				public bool use;
				public bool isUnique;
				public string type;
				public string selector;
				public string findMethod;
			}

			public static List<Configuration> GetConfigurations(TableGeneratorData data)
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
							classNameSeparator = folderData.classNameSeparator,
							classNameEraser = folderData.classNameEraser,
							classNameFormat = folderData.classNameFormat
						});
					}
				}

				return configurations;
			}

			public static string GetModelClassName(Configuration configuration, CsvHeader csvHeader)
			{
				string modelClassName = csvHeader.tableName;
				if (!string.IsNullOrEmpty(configuration.classNameSeparator)) {
					char separator = configuration.classNameSeparator[0];
					string[] words = modelClassName.Split(separator);
					modelClassName = string.Join(separator.ToString(), words.Select(_ => Utils.Text.ToUpper(_, 0)));
				}
				if (!string.IsNullOrEmpty(configuration.classNameEraser)) {
					modelClassName = Regex.Replace(modelClassName, configuration.classNameEraser, "", RegexOptions.IgnoreCase);
				}
				return configuration.classNameFormat.Replace("*", modelClassName);
			}

			public static bool IsValidateVariableType(string variableType)
			{
				if (string.IsNullOrEmpty(variableType)) return false;
				return Regex.IsMatch("int|long|float|string", variableType);
			}

			public static SecondaryInfo GetSecondaryInfo(CsvHeader csvHeader)
			{
				List<int> secondaryIndices = new List<int>();
				bool isUnique = true;
				for (int i = 0; i < csvHeader.attributes.Count; i++) {
					if (csvHeader.attributes[i].ToLower().Contains("secondary")) {
						secondaryIndices.Add(i);
						isUnique &= !csvHeader.attributes[i].ToLower().Contains("nonunique");
					}
				}

				List<(string type, string upperCamelName, string lowerCamelName)> secondaryProperties = secondaryIndices
					.Select(_ => {
						string upperCamel = string.Concat(csvHeader.columns[_].Split('_').Select(word => Utils.Text.ToUpper(word, 0)));
						return (csvHeader.types[_], upperCamel, Utils.Text.ToLower(upperCamel, 0));
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

				SecondaryInfo secondaryInfo = new SecondaryInfo
				{
					use = use,
					isUnique = isUnique,
					type = type,
					selector = selector,
					findMethod = findMethod
				};

				return secondaryInfo;
			}
		}
	}
}