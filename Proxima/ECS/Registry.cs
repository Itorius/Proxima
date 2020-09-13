using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Proxima.ECS
{
	public static class Registry
	{
		private static List<Entity> entities;
		private static Dictionary<long, Pool> pools;

		private static Entity Destroyed;

		static Registry()
		{
			entities = new List<Entity>();
			pools = new Dictionary<long, Pool>();

			Destroyed = Entity.Null;
		}

		internal static Pool<T> GetPool<T>() where T : struct
		{
			long id = ComponentType<T>.ID;

			if (pools.TryGetValue(id, out Pool pool)) return (Pool<T>)pool;

			pool = new Pool<T>();
			pools.Add(id, pool);
			return (Pool<T>)pool;
		}

		public static Entity CreateEntity(string? tag = null)
		{
			Entity entity;

			if (Destroyed == Entity.Null)
			{
				entity = new Entity((uint)entities.Count);
				entities.Add(entity);

				Debug.Assert(entity.ID < Entity.IndexMask);
			}
			else
			{
				uint index = Destroyed.Index;
				uint version = entities[(int)index].Version;
				Destroyed = new Entity(entities[(int)index].Index);
				entity = entities[(int)index] = new Entity(index, version);
			}

			entity.AddComponent(new TagComponent(tag ?? "Entity"));
			return entity;
		}

		public static void DestroyEntity(Entity entity)
		{
			Debug.Assert(entities.Contains(entity));

			uint version = entity.Version + 1;

			foreach (Pool pool in pools.Values.Where(pool => pool.Contains(entity))) pool.Remove(entity);

			uint entt = entity.Index;
			entities[(int)entt] = new Entity(Destroyed.ID | (version << Entity.IndexShift));
			Destroyed = new Entity(entt);
		}

		public static View<T1> View<T1>() where T1 : struct => new View<T1>();

		public static View<T1, T2> View<T1, T2>() where T1 : struct where T2 : struct => new View<T1, T2>();
		public static View<T1, T2, T3> View<T1, T2, T3>() where T1 : struct where T2 : struct where T3 : struct => new View<T1, T2, T3>();

		private static Dictionary<long, Group> groupCache = new Dictionary<long, Group>();

		// note: this should be only allowed for partial-owning groups
		// note: implement a builder pattern for that GroupBuilder.Own<T>().Include<K>().Exclude<L>();
		// public static Group<T1> Group<T1>() where T1 : struct
		// {
		// 	var id = ComponentType<T1>.id;
		// 	if (groupCache.TryGetValue(id, out Group group)) return (Group<T1>)group;
		//
		// 	group = new Group<T1>();
		// 	groupCache[id] = group;
		// 	return (Group<T1>)group;
		// }

		public static Group<T1, T2> Group<T1, T2>() where T1 : struct where T2 : struct
		{
			var id1 = ComponentType<T1>.ID;
			var id2 = ComponentType<T2>.ID;
			var id = id1 | id2;

			if (groupCache.TryGetValue(id, out Group group)) return (Group<T1, T2>)group;

			group = new Group<T1, T2>();
			groupCache[id] = group;
			return (Group<T1, T2>)group;
		}
	}
}