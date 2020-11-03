using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;

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

		public Vector4 ToRGB()
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

			return new Vector4(r, g, b, Alpha);

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

	public static class MathUtility
	{
		public static float Lerp(float a, float b, float value)
		{
			return a * (1f - value) + b * value;
		}

		public static float Sin(float angle, float min = 0f, float max = 1f)
		{
			return Lerp(min, max, MathF.Sin(Time.TotalUpdateTime) * 0.5f + 0.5f);
		}
	}

	public static class Utility
	{
		public static unsafe void MemoryCopy(void* src, void* dst, ulong size) => Buffer.MemoryCopy(src, dst, size, size);

		public static unsafe void MemoryCopy(IntPtr src, void* dst, ulong size) => Buffer.MemoryCopy(src.ToPointer(), dst, size, size);

		public static unsafe void MemoryCopy(byte[] src, void* dst, ulong size)
		{
			fixed (void* ptr = src) Buffer.MemoryCopy(ptr, dst, size, size);
		}

		public static T[] GetInternalArray<T>(this List<T> list)
		{
			FieldInfo fieldInfo = list.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
			return (T[])fieldInfo.GetValue(list);
		}

		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> predicate)
		{
			foreach (T element in collection) predicate(element);
		}

		public static void ForEach<T>(this IEnumerable<T> collection, Action<int, T> predicate)
		{
			int i = 0;
			foreach (T element in collection) predicate(i++, element);
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
	}
}