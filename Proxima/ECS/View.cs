using System;
using System.Collections;
using System.Collections.Generic;

namespace Proxima.ECS
{
	public abstract class View : IEnumerable<Entity>
	{
		public abstract IEnumerator<Entity> GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Each(Action<Entity> action)
		{
			using var enumerator = GetEnumerator();
			while (enumerator.MoveNext()) action(enumerator.Current);
		}
	}

	public sealed class View<T1> : View where T1 : struct
	{
		private Pool<T1> pool;

		internal View()
		{
			pool = Registry.GetPool<T1>();
		}

		public override IEnumerator<Entity> GetEnumerator() => pool.GetEnumerator();

		public T Get<T>(Entity entity) where T : struct => throw new NotImplementedException();
	}

	public sealed class View<T1, T2> : View where T1 : struct where T2 : struct
	{
		private Pool<T1> pool1;
		private Pool<T2> pool2;

		internal View()
		{
			pool1 = Registry.GetPool<T1>();
			pool2 = Registry.GetPool<T2>();
		}

		public override IEnumerator<Entity> GetEnumerator()
		{
			var (shortest, longest) = Pool.MinMax(pool1, pool2);

			foreach (Entity entity in shortest)
			{
				if (entity.Page < longest.sparse.Count && longest.sparse[entity.Page][entity.Offset] != Entity.Null.ID) yield return entity;
			}
		}

		public T Get<T>(Entity entity) where T : struct => throw new NotImplementedException();

		public (K1, K2) Get<K1, K2>(Entity entity) where K1 : struct where K2 : struct => throw new NotImplementedException();
	}

	public sealed class View<T1, T2, T3> : View where T1 : struct where T2 : struct where T3 : struct
	{
		private Pool<T1> pool1;
		private Pool<T2> pool2;
		private Pool<T3> pool3;

		internal View()
		{
			pool1 = Registry.GetPool<T1>();
			pool2 = Registry.GetPool<T2>();
			pool3 = Registry.GetPool<T3>();
		}

		public override IEnumerator<Entity> GetEnumerator()
		{
			var (min, medium, max) = Pool.MinMax(pool1, pool2, pool3);

			foreach (Entity entity in min)
			{
				if (entity.Page < medium.sparse.Count && medium.sparse[entity.Page][entity.Offset] != Entity.Null.ID)
					if (entity.Page < max.sparse.Count && max.sparse[entity.Page][entity.Offset] != Entity.Null.ID)
						yield return entity;
			}
		}

		public T Get<T>(Entity entity) where T : struct => throw new NotImplementedException();

		public (K1, K2, K3) Get<K1, K2, K3>(Entity entity) where K1 : struct where K2 : struct where K3 : struct => throw new NotImplementedException();
	}
}