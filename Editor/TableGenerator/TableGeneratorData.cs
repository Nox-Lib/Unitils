using UnityEngine;

namespace Unitils
{
	public class TableGeneratorData : ScriptableObject
	{
		[SerializeField] private string inputFolder;
		public string InputFolder => this.inputFolder;

		[SerializeField] private string classGenerateFolder;
		public string ClassGenerateFolder => this.classGenerateFolder;

		[SerializeField] private string dataGenerateFolder;
		public string DataGenerateFolder => this.dataGenerateFolder;

		[SerializeField] private bool isWritableTable;
		public bool IsWritableTable => this.isWritableTable;

		[SerializeField] private string writableTablePathMatchPattern;
		public string WritableTablePathMatchPattern => this.writableTablePathMatchPattern;

		public TableGeneratorData()
		{
			this.inputFolder = "../TableGenerator";
			this.classGenerateFolder = "Tables";
			this.dataGenerateFolder = "Tables";
		}
	}
}