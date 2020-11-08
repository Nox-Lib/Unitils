using UnityEngine;

namespace Unitils
{
	[ExecuteAlways]
	public class GizmosDrawAnnulus : MonoBehaviour
	{
		private const int DIVISION_COUNT = 36;

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

		[SerializeField] private float innerRadius = 1f;
		public float InnerRadius {
			get { return this.innerRadius; }
			set { this.innerRadius = value; }
		}

		[SerializeField] private float outerRadius = 1.5f;
		public float OuterRadius {
			get { return this.outerRadius; }
			set { this.outerRadius = value; }
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

			Color beforeColor = Gizmos.color;
			Gizmos.color = this.Color;

			if (this.IsShowRay) {
				Gizmos.DrawRay(originPosition, this.CenterOffset);
			}
			this.DrawGizmoCircumference(this.InnerRadius);
			this.DrawGizmoCircumference(this.OuterRadius);

			Gizmos.color = beforeColor;
		}

		private void DrawGizmoCircumference(float radius)
		{
			Vector3 originPosition = this.Origin.position + this.OriginOffset;
			Vector3 centerPosition = originPosition + this.CenterOffset;

			for (int i = 0; i < DIVISION_COUNT; i++) {
				Vector3 from = originPosition + centerOffset;
				Vector3 to = from;
				float radian = Mathf.PI * 2 / DIVISION_COUNT;
				from.x += Mathf.Cos(radian * i) * radius;
				from.z += Mathf.Sin(radian * i) * radius;
				to.x += Mathf.Cos(radian * (i + 1)) * radius;
				to.z += Mathf.Sin(radian * (i + 1)) * radius;
				Gizmos.DrawLine(from, to);
			}
		}
		#endif
	}
}