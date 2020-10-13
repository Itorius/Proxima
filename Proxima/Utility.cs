using System;
using System.Collections.Generic;
using Vortice.Mathematics;

namespace Proxima
{
	public struct HslColor
	{
		public float Hue, Saturation, Lightness, Alpha;

		public HslColor(float hue, float saturation, float lightness, float alpha = 1f)
		{
			Hue = hue;
			Saturation = saturation;
			Lightness = lightness;
			Alpha = alpha;
		}

		public Color4 ToRGB()
		{
			float r, g, b;

			if (Saturation == 0) r = g = b = Lightness;
			else
			{
				float q = Lightness < 0.5f ? Lightness * (1 + Saturation) : Lightness + Saturation - Lightness * Saturation;
				float p = 2f * Lightness - q;
				r = hue2rgb(p, q, Hue + 0.33333334f);
				g = hue2rgb(p, q, Hue);
				b = hue2rgb(p, q, Hue - 0.33333334f);
			}

			return new Color4(r, g, b, Alpha);

			static float hue2rgb(float p, float q, float t)
			{
				if (t < 0f) t += 1;
				if (t > 1f) t -= 1;
				if (t < 1 / 6f) return p + (q - p) * 6f * t;
				if (t < 0.5f) return q;
				if (t < 2 / 3f) return p + (q - p) * (2 / 3f - t) * 6f;
				return p;
			}
		}
	}

	public static class Utility
	{
		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> predicate)
		{
			foreach (T VARIABLE in collection) predicate(VARIABLE);
		}
		
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