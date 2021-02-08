using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unitils
{
	public class TableGeneratorData : ScriptableObject
	{
		[Serializable]
		public class FolderData
		{
			public bool enabled = true;
			public string path;
			public string folderName;
			public bool isWritableTable;
			public string separator = "_";
			public string classNameEraser = "_";
			public string classNameFormat = "*";
		}

		[SerializeField] private string inputFolder;
		public string InputFolder => this.inputFolder;

		[SerializeField] private string classOutputFolder;
		public string ClassOutputFolder => this.classOutputFolder;

		[SerializeField] private string dataOutputFolder;
		public string DataOutputFolder => this.dataOutputFolder;

		[SerializeField] private bool isDataFileNameToMD5;
		public bool IsDataFileNameToMD5 => this.isDataFileNameToMD5;

		[SerializeField] private Define.EncryptionType encryptionType;
		public Define.EncryptionType EncryptionType => this.encryptionType;

		[SerializeField] private string encryptionAesIv;
		public string EncryptionAesIv => this.encryptionAesIv;

		[SerializeField] private string encryptionAesKey;
		public string EncryptionAesKey => this.encryptionAesKey;

		[SerializeField] private List<FolderData> folders;
		public List<FolderData> Folders => this.folders;


		public TableGeneratorData()
		{
			this.inputFolder = "../TableGenerator";
			this.classOutputFolder = "Tables";
			this.dataOutputFolder = "StreamingAssets/Tables";
			this.encryptionAesIv = Utils.Security.GeneratePassword(16);
			this.encryptionAesKey = Utils.Security.GeneratePassword(16);
			this.folders = new List<FolderData>();
		}
	}
}