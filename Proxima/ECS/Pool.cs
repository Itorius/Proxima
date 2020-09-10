using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable PossibleNullReferenceException

namespace Proxima.ECS
{
	public abstract class Pool
	{
		public const int EntitiesPerPage = 10000;

		public int Count => packed.Count;

		internal List<uint[]?> sparse = null!;
		protected List<Entity> packed = null!;

		protected bool Owned;

		protected uint[] Assure(int position)
		{
			if (position >= sparse.Count) sparse.AddRange(Enumerable.Repeat<uint[]>(null, position - sparse.Count + 1));

			sparse[position] ??= Enumerable.Repeat(Entity.Null.ID, EntitiesPerPage).ToArray();

			return sparse[position];
		}

		public bool Contains(Entity entity)
		{
			return entity.Page < sparse.Count && sparse[(int)entity.Page][entity.Offset] != Entity.Null.ID;
		}

		public uint GetIndex(Entity entity) => sparse[(int)entity.Page][entity.Offset];

		public List<Entity>.Enumerator GetEnumerator() => packed.GetEnumerator();

		public static (Pool min, Pool max) MinMax(Pool pool1, Pool pool2) => pool1.Count <= pool2.Count ? (pool1, pool2) : (pool2, pool1);

		public static (Pool min, Pool medium, Pool max) MinMax(Pool pool1, Pool pool2, Pool pool3)
		{
			var array = new[] { pool1, pool2, pool3};
			
			Array.Sort(array, (pool, pool1) => pool.Count < pool1.Count ? -1 : 1);

			return (array[0], array[1], array[2]);
		}

		public Entity this[int i] => packed[i];

		public abstract void Swap(Entity lhs, Entity rhs);

		public abstract void Remove(Entity entity);
	}

	public class Pool<T> : Pool where T : struct
	{
		public readonly List<T> component;
		public event Action<Entity>? OnEntityAdded;
		public event Action<Entity>? OnEntityRemoved;

		public Pool()
		{
			// note: use custom types?
			sparse = new List<uint[]>();
			packed = new List<Entity>();
			component = new List<T>();
		}

		public void ShrinkToFit()
		{
			if (packed.Count <= 0) sparse.Clear();

			sparse.TrimExcess();
			packed.TrimExcess();
			component.TrimExcess();
		}

		public T Add(Entity entity, T component)
		{
			Assure(entity.Page)[entity.Offset] = (uint)packed.Count; // this assigns the position of the entity in the packed array

			packed.Add(entity);
			this.component.Add(component);

			OnEntityAdded?.Invoke(entity);

			return component;
		}

		public override void Remove(Entity entity)
		{
			// int curr = entity.Page;
			// int pos = entity.Offset;
			// packed[(int)sparse[curr][pos]] = packed.Last();
			// sparse[packed.Last().Page][packed.Last().Offset] = sparse[curr][pos];
			// sparse[curr][pos] = Entity.Null;
			//
			// // this is not correct
			//
			//
			//
			// component.RemoveAt(component.Count - 1);
			// packed.Remove(entity);
			//
			// OnEntityRemoved?.Invoke(entity);
		}

		public override void Swap(Entity lhs, Entity rhs)
		{
			if (lhs == rhs) return;
			
			int from = (int)sparse[lhs.Page][lhs.Offset];
			int to = (int)sparse[rhs.Page][rhs.Offset];

			Entity temp = packed[from];
			packed[from] = packed[to];
			packed[to] = temp;

			T tempComponent = component[from];
			component[from] = component[to];
			component[to] = tempComponent;

			sparse[lhs.Page][lhs.Offset] = (uint)to;
			sparse[rhs.Page][rhs.Offset] = (uint)@from;
		}

		public void Clear()
		{
			sparse.Clear();
			packed.Clear();
			component.Clear();
		}

		public T Get(Entity entity) => component[(int)sparse[entity.Page][entity.Offset]];

		public bool TryGet(Entity entity, out T component)
		{
			if (Contains(entity))
			{
				component = Get(entity);
				return true;
			}

			component = default;
			return false;
		}

		public Pool<T> Own()
		{
			if (Owned) throw new Exception($"Pool<{typeof(T).FullName}> is already owned");

			Owned = true;
			return this;
		}
	}
}