using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Unitils
{
	public interface ILottery
	{
		int LotteryWeight { get; }
	}

	public static class Lottery
	{
		public static T Draw<T>(IEnumerable<T> items) where T : ILottery
		{
			int rand = Random.Range(0, items.Sum(x => x.LotteryWeight));
			int sum = 0;
			foreach (T item in items) {
				sum += item.LotteryWeight;
				if (sum >= rand) {
					return item;
				}
			}
			return default;
		}

		public static int Draw(params int[] weights)
		{
			int rand = Random.Range(0, weights.Sum());
			int sum = 0;
			for (int i = 0; i < weights.Length; i++) {
				sum += weights[i];
				if (sum >= rand) {
					return i;
				}
			}
			return 0;
		}

		public static int Draw(IEnumerable<int> weights)
		{
			int rand = Random.Range(0, weights.Sum());
			int sum = 0;
			int index = 0;
			foreach (int weight in weights) {
				sum += weight;
				if (sum >= rand) {
					return index;
				}
				++index;
			}
			return 0;
		}
	}
}