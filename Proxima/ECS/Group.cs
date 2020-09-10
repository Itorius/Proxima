using System;
using System.Collections;
using System.Collections.Generic;

namespace Proxima.ECS
{
	public abstract class Group : IEnumerable<Entity>
	{
		public abstract IEnumerator<Entity> GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public void Each(Action<Entity> action)
		{
			using var enumerator = GetEnumerator();
			while (enumerator.MoveNext()) action(enumerator.Current);
		}
	}

	// public sealed class Group<T1> : Group where T1 : struct
	// {
	// 	private Pool<T1> pool;
	//
	// 	internal Group()
	// 	{
	// 		pool = Registry.GetPool<T1>().Own();
	// 	}
	//
	// 	public override IEnumerator<Entity> GetEnumerator() => pool.GetEnumerator();
	//
	// 	public T Get<T>(Entity entity) where T : struct => throw new NotImplementedException();
	// }

	public sealed class Group<T1, T2> : Group where T1 : struct where T2 : struct
	{
		private Pool<T1> pool1;
		private Pool<T2> pool2;
		private int groupIndex;
		
		internal Group()
		{
			pool1 = Registry.GetPool<T1>().Own();
			pool1.OnEntityAdded += EntityAdded;
			pool2 = Registry.GetPool<T2>().Own();
			pool2.OnEntityAdded += EntityAdded;

			var (shortest, longest) = Pool.MinMax(pool1, pool2);
			
			for (int i = 0; i < shortest.Count; i++)
			{
				Entity e = shortest[i];
				if (longest.Contains(e))
				{
					shortest.Swap(shortest[groupIndex], shortest[i]);
					longest.Swap(longest[groupIndex], longest[(int)longest.GetIndex(e)]);
					
					groupIndex++;
				}
			}
		}

		private void EntityAdded(Entity entity)
		{
		}

		public override IEnumerator<Entity> GetEnumerator()
		{
			for (int i = 0; i < groupIndex; i++)
			{
				yield return pool1[i];
			}
		}

		public T Get<T>(Entity entity) where T : struct => throw new NotImplementedException();

		public (K1, K2) Get<K1, K2>(Entity entity) where K1 : struct where K2 : struct => throw new NotImplementedException();
	}
}