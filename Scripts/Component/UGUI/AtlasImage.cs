using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

namespace Unitils
{
	public class AtlasImage : Image
	{
		[SerializeField] private SpriteAtlas atlas;
		public SpriteAtlas Atlas {
			get { return this.atlas; }
			set { this.atlas = value; }
		}

		[SerializeField, ReadOnly] private string spriteName;
		public string SpriteName {
			get { return this.spriteName; }
			set {
				this.spriteName = value;
				this.SetSprite();
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.SetSprite();
		}

		private void SetSprite()
		{
			this.sprite = null;
			if (this.Atlas != null && !string.IsNullOrEmpty(this.spriteName)) {
				this.sprite = this.Atlas.GetSprite(this.spriteName);
			}
		}
	}
}