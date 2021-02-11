using UnityEngine;

namespace Unitils
{
	[ExecuteAlways]
	public class GizmosDrawSphere : MonoBehaviour
	{
		[SerializeField] private Color color = Color.white;
		public Color Color {
			get { return this.color; }
			set { this.color = value; }
		}

		[SerializeField] private Transform origin;
		public Transform Origin {
			get { return this.origin; }
			set { this.origin = value; }
		}

		[SerializeField] private float radius = 0.5f;
		public float Radius {
			get { return this.radius; }
			set { this.radius = value; }
		}

		[SerializeField] private Vector3 originOffset;
		public Vector3 OriginOffset {
			get { return this.originOffset; }
			set { this.originOffset = value; }
		}

		[SerializeField] private Vector3 centerOffset;
		public Vector3 CenterOffset {
			get { return this.centerOffset; }
			set { this.centerOffset = value; }
		}

		[SerializeField] private bool isWire = true;
		public bool IsWire {
			get { return this.isWire; }
			set { this.isWire = value; }
		}

		[SerializeField] private bool isShowRay;
		public bool IsShowRay {
			get { return this.isShowRay; }
			set { this.isShowRay = value; }
		}


		#if UNITY_EDITOR

		private void Awake()
		{
			if (this.Origin == null) {
				this.Origin = this.transform;
			}
		}

		private void OnDrawGizmos()
		{
			if (this.Origin == null) return;

			Vector3 originPosition = this.Origin.position + this.OriginOffset;
			Vector3 centerPosition = originPosition + this.CenterOffset;

			Color beforeColor = Gizmos.color;
			Gizmos.color = this.Color;

			if (this.IsShowRay) {
				Gizmos.DrawRay(originPosition, this.CenterOffset);
			}

			if (this.IsWire) {
				Gizmos.DrawWireSphere(centerPosition, this.Radius);
			}
			else {
				Gizmos.DrawSphere(centerPosition, this.Radius);
			}

			Gizmos.color = beforeColor;
		}
		#endif
	}
}