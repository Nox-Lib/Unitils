using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unitils
{
	public static class TransformExtensions
	{
		public static void DestroyChildren(this Transform self)
		{
			foreach (Transform child in self) {
				Object.Destroy(child.gameObject);
			}
		}

		public static void DestroyChildren(this Transform self, IEnumerable<string> ignoreNames)
		{
			foreach (Transform child in self) {
				if (!ignoreNames.Any(_ => child.name.Contains(_))) Object.Destroy(child.gameObject);
			}
		}


		#region Position

		public static void SetPosition(this Transform self, float x, float y, float z)
		{
			self.position = new Vector3(x, y, z);
		}

		public static void SetPositionX(this Transform self, float x)
		{
			self.position = new Vector3(x, self.position.y, self.position.z);
		}

		public static void SetPositionY(this Transform self, float y)
		{
			self.position = new Vector3(self.position.x, y, self.position.z);
		}

		public static void SetPositionZ(this Transform self, float z)
		{
			self.position = new Vector3(self.position.x, self.position.y, z);
		}

		public static void AddPositionX(this Transform self, float x)
		{
			self.position = new Vector3(self.position.x + x, self.position.y, self.position.z);
		}

		public static void AddPositionY(this Transform self, float y)
		{
			self.position = new Vector3(self.position.x, self.position.y + y, self.position.z);
		}

		public static void AddPositionZ(this Transform self, float z)
		{
			self.position = new Vector3(self.position.x, self.position.y, self.position.z + z);
		}
		#endregion


		#region LocalPosition

		public static void SetLocalPosition(this Transform self, float x, float y)
		{
			self.localPosition = new Vector3(x, y, self.localPosition.z);
		}

		public static void SetLocalPosition(this Transform self, float x, float y, float z)
		{
			self.localPosition = new Vector3(x, y, z);
		}

		public static void SetLocalPositionX(this Transform self, float x)
		{
			self.localPosition = new Vector3(x, self.localPosition.y, self.localPosition.z);
		}

		public static void SetLocalPositionY(this Transform self, float y)
		{
			self.localPosition = new Vector3(self.localPosition.x, y, self.localPosition.z);
		}

		public static void SetLocalPositionZ(this Transform self, float z)
		{
			self.localPosition = new Vector3(self.localPosition.x, self.localPosition.y, z);
		}

		public static void AddLocalPositionX(this Transform self, float x)
		{
			self.localPosition = new Vector3(self.localPosition.x + x, self.localPosition.y, self.localPosition.z);
		}

		public static void AddLocalPositionY(this Transform self, float y)
		{
			self.localPosition = new Vector3(self.localPosition.x, self.localPosition.y + y, self.localPosition.z);
		}

		public static void AddLocalPositionZ(this Transform self, float z)
		{
			self.localPosition = new Vector3(self.localPosition.x, self.localPosition.y, self.localPosition.z + z);
		}
		#endregion


		#region LocalRotation

		public static void SetLocalRotationX(this Transform self, float x)
		{
			self.localRotation = Quaternion.Euler(x, self.eulerAngles.y, self.eulerAngles.z);
		}

		public static void SetLocalRotationY(this Transform self, float y)
		{
			self.localRotation = Quaternion.Euler(self.eulerAngles.x, y, self.eulerAngles.z);
		}

		public static void SetLocalRotationZ(this Transform self, float z)
		{
			self.localRotation = Quaternion.Euler(self.eulerAngles.x, self.eulerAngles.y, z);
		}
		#endregion


		#region LocalScale

		public static void SetLocalScale(this Transform self, float scale)
		{
			self.localScale = new Vector3(scale, scale, scale);
		}

		public static void SetLocalScale(this Transform self, float x, float y, float z)
		{
			self.localScale = new Vector3(x, y, z);
		}

		public static void SetLocalScaleX(this Transform self, float x)
		{
			self.localScale = new Vector3(x, self.localScale.y, self.localScale.z);
		}

		public static void SetLocalScaleY(this Transform self, float y)
		{
			self.localScale = new Vector3(self.localScale.x, y, self.localScale.z);
		}

		public static void SetLocalScaleZ(this Transform self, float z)
		{
			self.localScale = new Vector3(self.localScale.x, self.localScale.y, z);
		}
		#endregion
	}
}