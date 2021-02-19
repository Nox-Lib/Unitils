namespace Unitils
{
	public static class TableGeneratorTemplate
	{
		public static class Model
		{
			public const string CLASS =
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

			public const string READONLY_PROPERTY =
				"\t\t/// <summary>\n" +
				"\t\t/// {3}\n" +
				"\t\t/// </summary>\n" +
				"\t\t[SerializeField] private {0} {1};\n" +
				"\t\tpublic {0} {2} => this.{1};\n\n";

			public const string WRITABLE_PROPERTY =
				"\t\t/// <summary>\n" +
				"\t\t/// {3}\n" +
				"\t\t/// </summary>\n" +
				"\t\t[SerializeField] private {0} {1};\n" +
				"\t\tpublic {0} {2} {{ get {{ return this.{1}; }} set {{ this.{1} = value; }} }}\n\n";
		}

		public static class Table
		{
			public const string CLASS =
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

			public const string PRIMARY_PROPERTY =
				"\t\tprivate readonly Func<{0}, {1}> primaryKeySelector;\n\n";

			public const string SECONDARY_PROPERTY =
				"\t\tprivate readonly Func<{0}, {1}> secondaryIndexSelector;\n" +
				"\t\tprivate readonly {2}[] secondaryIndex;\n\n";

			public const string CONSTRUCTOR =
				"\t\tpublic {0}({1}[] source) : base(source)\n" +
				"\t\t{{\n" +
				"\t\t\tthis.primaryKeySelector = _ => _.{2};" +
//				"\t\t\tthis.source = this.CloneAndSortBy(this.primaryKeySelector, Comparer<{3}>.Default);\n" +
				"\t\t}}\n\n";

			public const string CONSTRUCTOR_TEMPLATE_FOR_SECONDARY =
				"\t\tpublic {0}({1}[] source) : base(source)\n" +
				"\t\t{{\n" +
				"\t\t\tthis.primaryKeySelector = _ => _.{2};\n" +
//				"\t\t\t//this.source = this.CloneAndSortBy(this.primaryKeySelector, Comparer<{3}>.Default);\n" +
				"\t\t\tthis.secondaryIndexSelector = _ => {4};\n" +
				"\t\t\tthis.secondaryIndex = this.CloneAndSortBy(this.secondaryIndexSelector, Comparer<{5}>.Default);\n" +
				"\t\t}}\n\n";

			public const string FIND_BY_METHOD =
				"\t\tpublic {0} FindBy{1}({2} {3})\n" +
				"\t\t{{\n" +
				"\t\t\treturn this.FindUnique(this.source, this.primaryKeySelector, Comparer<{2}>.Default, {3});\n" +
				"\t\t}}\n\n";

			public const string FIND_BY_SECONDARY_METHOD =
				"\t\tpublic {0} FindBy{1}({2} key)\n" +
				"\t\t{{\n" +
				"\t\t\treturn this.FindUnique(this.secondaryIndex, this.secondaryIndexSelector, Comparer<{2}>.Default, key);\n" +
				"\t\t}}\n\n";

			public const string FIND_BY_SECONDARY_NONUNIQUE_METHOD =
				"\t\tpublic RangeList<{0}> FindBy{1}({2} key)\n" +
				"\t\t{{\n" +
				"\t\t\treturn this.FindMany(this.secondaryIndex, this.secondaryIndexSelector, Comparer<{2}>.Default, key);\n" +
				"\t\t}}\n\n";

			public const string FIND_RANGE_METHOD =
				"\t\tpublic RangeList<{0}> FindRangeBy{1}({2} min, {2} max)\n" +
				"\t\t{{\n" +
				"\t\t\treturn this.FindUniqueRange(this.secondaryIndex, this.secondaryIndexSelector, Comparer<{2}>.Default, min, max);\n" +
				"\t\t}}\n\n";
		}

		public static class DB
		{
			public const string CLASS =
				"namespace {0}\n" +
				"{{\n" +
				"\tpublic partial class {1}\n" +
				"\t{{\n" +
				"\t\tprivate static {1} instance = null;\n" +
				"\t\tpublic static {1} Instance => instance ??= new {1}();\n\n" +
				"{2}" +
				"\t}}\n" +
				"}}";

			public const string PROPERTY =
				"\t\tpublic {0} {0} {{ get; private set; }}\n";
		}
	}
}