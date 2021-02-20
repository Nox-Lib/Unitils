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

		public static class RTable
		{
			public const string CLASS =
				"using System;\n" +
				"using System.Collections.Generic;\n" +
				"using Unitils;\n" +
				"\n" +
				"namespace {0}\n" +
				"{{\n" +
				"\tpublic class {1} : ReadonlyTable<{2}>\n" +
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
				"\t\t\tthis.primaryKeySelector = _ => _.{2};\n" +
//				"\t\t\tthis.source = this.CloneAndSortBy(this.primaryKeySelector, Comparer<{3}>.Default);\n" +
				"\t\t}}\n\n";

			public const string CONSTRUCTOR_ALSO_SECONDARY =
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
		}

		public static class WTable
		{
			public const string CLASS =
				"using System;\n" +
				"using System.Collections.Generic;\n" +
				"using Unitils;\n" +
				"\n" +
				"namespace {0}\n" +
				"{{\n" +
				"\tpublic class {1} : WritableTable<{2}>\n" +
				"\t{{\n" +
				"{3}\n" +
				"\t}}\n" +
				"}}";

			public const string PRIMARY_PROPERTY =
				"\t\tprivate readonly Func<{0}, {1}> primaryKeySelector;\n\n";

			public const string SECONDARY_PROPERTY =
				"\t\tprivate readonly Func<{0}, {1}> secondaryIndexSelector;\n" +
				"\t\tprivate readonly List<{2}> secondaryIndex;\n\n";

			public const string CONSTRUCTOR =
				"\t\tpublic {0}(List<{1}> source = null) : base(source)\n" +
				"\t\t{{\n" +
				"\t\t\tthis.primaryKeySelector = _ => _.{2};\n" +
//				"\t\t\tthis.source = this.CloneAndSortBy(this.primaryKeySelector, Comparer<{3}>.Default);\n" +
				"\t\t}}\n\n";

			public const string CONSTRUCTOR_ALSO_SECONDARY =
				"\t\tpublic {0}(List<{1}> source = null) : base(source)\n" +
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
				"\t\tpublic IEnumerable<{0}> FindBy{1}({2} key)\n" +
				"\t\t{{\n" +
				"\t\t\treturn this.FindMany(this.secondaryIndex, this.secondaryIndexSelector, Comparer<{2}>.Default, key);\n" +
				"\t\t}}\n\n";

			public const string ADD_METHOD =
				"\t\tpublic void Add({0} item)\n" +
				"\t\t{{\n" +
				"\t\t\tthis.Insert(item, this.source, this.primaryKeySelector, Comparer<{1}>.Default, true);\n" +
				"\t\t}}\n\n";

			public const string ADD_METHOD_ALSO_SECONDARY =
				"\t\tpublic void Add({0} item)\n" +
				"\t\t{{\n" +
				"\t\t\tthis.Insert(item, this.source, this.primaryKeySelector, Comparer<{1}>.Default, true);\n" +
				"\t\t\tthis.Insert(item, this.secondaryIndex, this.secondaryIndexSelector, Comparer<{2}>.Default, {3});\n" +
				"\t\t}}\n\n";

			public const string REMOVE_KEY_METHOD =
				"\t\tpublic void Remove(int {0})\n" +
				"\t\t{{\n" +
				"\t\t\tthis.Remove(this.FindUnique(this.source, this.primaryKeySelector, Comparer<{1}>.Default, {0}, false));\n" +
				"\t\t}}\n\n";

			public const string REMOVE_ITEM_METHOD =
				"\t\tpublic void Remove({0} item)\n" +
				"\t\t{{\n" +
				"\t\t\tthis.source.Remove(item);\n" +
				"\t\t}}\n\n";

			public const string REMOVE_ITEM_METHOD_ALSO_SECONDARY =
				"\t\tpublic void Remove({0} item)\n" +
				"\t\t{{\n" +
				"\t\t\tthis.source.Remove(item);\n" +
				"\t\t\tthis.secondaryIndex.Remove(item);\n" +
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