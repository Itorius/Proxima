using System;
using System.Collections.Generic;

namespace Proxima
{
	public static class Utility
	{
		public static IEnumerable<int> IndicesOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			int i = 0;
			foreach (T element in collection)
			{
				if (predicate(element)) yield return i;
				i++;
			}
		}

		public static void RemoveLast<T>(this List<T> collection) => collection.RemoveAt(collection.Count - 1);

		public static double Max(this IEnumerable<double> enumerable, out int index)
		{
			double max = double.MinValue;
			index = 0;

			int i = 0;
			foreach (double comparable in enumerable)
			{
				if (comparable > max)
				{
					max = comparable;
					index = i;
				}

				i++;
			}

			return max;
		}

		public static double Min(this IEnumerable<double> enumerable, out int index)
		{
			double min = double.MaxValue;
			index = 0;

			int i = 0;
			foreach (double comparable in enumerable)
			{
				if (comparable < min)
				{
					min = comparable;
					index = i;
				}

				i++;
			}

			return min;
		}
	}
}