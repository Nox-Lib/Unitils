using System;
using UnityEngine;

namespace Unitils
{
	public class Smoothing
	{
		private readonly float[] samplingArray;
		private readonly float[] factorArray;
		private readonly int samplingCount;

		private float smoothingFactor;

		public float SmoothValue { get; private set; }

		public Smoothing(int samplingCount, float currentValue)
		{
			this.samplingCount = Mathf.Max(samplingCount, 2);

			this.samplingArray = new float[samplingCount];
			this.factorArray = new float[this.samplingCount];

			this.Reset(currentValue);
			this.SmoothValue = currentValue;

			this.SetFactor(0.5f);
		}

		public void SetFactor(float smoothingFactor)
		{
			this.smoothingFactor = Mathf.Clamp01(smoothingFactor);

			float sum = 0f;
			float t;
			for (int i = 0; i < this.factorArray.Length; i++) {
				t = (float)(i + 1) / this.factorArray.Length;
				this.factorArray[i] = Utils.Math.SplinePoint(0f, 1f, t, this.smoothingFactor) - sum;
				sum += this.factorArray[i];
			}
		}

		public float Sample(float currentValue)
		{
			for (int i = this.samplingArray.Length - 1; i > 0; i--) {
				this.samplingArray[i] = this.samplingArray[i - 1];
			}
			this.samplingArray[0] = currentValue;

			this.SmoothValue = 0f;
			for (int i = 0; i < this.samplingArray.Length; i++) {
				this.SmoothValue += this.samplingArray[i] * this.factorArray[i];
			}
			return this.SmoothValue;
		}

		public void Reset(float currentValue)
		{
			for (int i = 0; i < this.samplingArray.Length; i++) {
				this.samplingArray[i] = currentValue;
			}
		}
	}


	public class Smoothing<T>
	{
		private readonly T[] samplingArray;
		private readonly float[] factorArray;
		private readonly int samplingCount;

		private float smoothingFactor;

		private readonly Func<T[], float[], T> onSmoothMultiple;

		public T SmoothValue { get; private set; }

		public Smoothing(int samplingCount, T currentValue, Func<T[], float[], T> onSmoothMultiple)
		{
			this.samplingCount = Mathf.Max(samplingCount, 2);
			this.onSmoothMultiple = onSmoothMultiple;

			this.samplingArray = new T[samplingCount];
			this.factorArray = new float[this.samplingCount];

			this.Reset(currentValue);
			this.SmoothValue = currentValue;

			this.SetFactor(0.5f);
		}

		public void SetFactor(float smoothingFactor)
		{
			this.smoothingFactor = Mathf.Clamp01(smoothingFactor);

			float sum = 0f;
			float t;
			for (int i = 0; i < this.factorArray.Length; i++) {
				t = (float)(i + 1) / this.factorArray.Length;
				this.factorArray[i] = Utils.Math.SplinePoint(0f, 1f, t, this.smoothingFactor) - sum;
				sum += this.factorArray[i];
			}
		}

		public T Sample(T currentValue)
		{
			for (int i = this.samplingArray.Length - 1; i > 0; i--) {
				this.samplingArray[i] = this.samplingArray[i - 1];
			}
			this.samplingArray[0] = currentValue;

			this.SmoothValue = this.onSmoothMultiple(this.samplingArray, this.factorArray);
			return this.SmoothValue;
		}

		public void Reset(T currentValue)
		{
			for (int i = 0; i < this.samplingArray.Length; i++) {
				this.samplingArray[i] = currentValue;
			}
		}
	}
}