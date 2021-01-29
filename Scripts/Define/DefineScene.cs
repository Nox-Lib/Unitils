using System;

namespace Unitils
{
	public static partial class Define
	{
		[Serializable]
		public partial class SceneType : Enumeration
		{
			public SceneType(int id, string name) : base(id, name) {}

			public static readonly SceneType Unitils = new SceneType(0, "Unitils");
		}
	}
}