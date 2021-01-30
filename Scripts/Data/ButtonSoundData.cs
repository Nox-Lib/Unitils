using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unitils
{
	public class ButtonSoundData : ScriptableObject
	{
		private static ButtonSoundData instance = null;
		public static ButtonSoundData Instance {
			get {
				return instance = instance ??= Resources.Load<ButtonSoundData>(Define.BUTTON_SOUND_DATA);
			}
		}

		[Serializable]
		public class Pair
		{
			public Define.ButtonSoundType type;
			public string soundName;
		}

		[SerializeField]
		private List<Pair> list;
		public List<Pair> List => this.list;

		public ButtonSoundData()
		{
			this.list = new List<Pair>();
			Array types = Enum.GetValues(typeof(Define.ButtonSoundType));
			for (int i = 0; i < types.Length; i++) {
				Define.ButtonSoundType enumType = (Define.ButtonSoundType)types.GetValue(i);
				if (enumType == Define.ButtonSoundType.None) continue;
				this.list.Add(new Pair { type = enumType, soundName = "Sounds/sample" });
			}
		}

		public string Get(Define.ButtonSoundType type)
		{
			Pair pair = this.List.FirstOrDefault(_ => _.type == type);
			return pair?.soundName;
		}

		public IEnumerable<string> GetSoundNames()
		{
			return this.List.Select(_ => _.soundName);
		}
	}
}