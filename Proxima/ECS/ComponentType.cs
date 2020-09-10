using System;
using System.Collections.Generic;

namespace Proxima.ECS
{
	internal static class ComponentTypeCache
	{
		private static long NextID = 1;
		private static Dictionary<Type, long> typeMap = new Dictionary<Type, long>();

		public static long GetID<T>()
		{
			if (typeMap.TryGetValue(typeof(T), out long id)) return id;

			id = NextID;
			NextID <<= 1;
			typeMap.Add(typeof(T), id);
			return id;
		}
	}

	public static class ComponentType<T>
	{
		public static readonly long ID = ComponentTypeCache.GetID<T>();
	}
}