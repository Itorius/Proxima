using System;
using System.Xml;

namespace Proxima.ECS
{
	public readonly struct TagComponent
	{
		public readonly string Tag;

		public TagComponent(string tag) => Tag = tag;

		public override string ToString() => Tag;
	}

	public readonly struct Entity
	{
		public static readonly Entity Null = new Entity(IndexMask);

		public const uint IndexMask = 0xFFFFF;
		public const uint VersionMask = 0xFFF;
		public const int IndexShift = 20;

		public readonly uint ID;

		public int Page => (int)(ID / Pool.EntitiesPerPage);
		public int Offset => (int)(ID % Pool.EntitiesPerPage);

		public uint Version => ID >> IndexShift;
		public uint Index => ID & IndexMask;

		internal Entity(uint id) => ID = id;

		internal Entity(uint index, uint version)
		{
			ID = index;
			ID |= version << IndexShift;
		}

		public T AddComponent<T>() where T : struct => Registry.GetPool<T>().Add(this, new T());

		public T AddComponent<T>(params object[] args) where T : struct => throw new NotImplementedException();

		public T AddComponent<T>(T component) where T : struct => Registry.GetPool<T>().Add(this, component);

		public bool HasComponent<T>() where T : struct => Registry.GetPool<T>().Contains(this);

		public bool TryGetComponent<T>(out T component) where T : struct => Registry.GetPool<T>().TryGet(this, out component);

		// todo: RemoveComponent

		#region Equality members
		public static bool operator ==(Entity e1, Entity? e2)
		{
			if (e2 == null)
			{
				return (e1.ID & IndexMask) == IndexMask;
			}

			return e1.ID == e2.Value.ID;
		}

		public static bool operator !=(Entity e1, Entity? e2) => !(e1 == e2);

		public bool Equals(Entity other) => ID == other.ID;

		public override bool Equals(object obj) => obj is Entity other && Equals(other);

		public override int GetHashCode() => (int)ID;
		#endregion

		public override string ToString() => Index == IndexMask ? "Entity [null]" : $"Entity [index: {Index}, version: {Version}]";
	}
}