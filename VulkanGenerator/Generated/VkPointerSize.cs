using System;
using System.Runtime.InteropServices;

namespace Fireburst
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct VkPointerSize : IEquatable<VkPointerSize>
	{
		private readonly UIntPtr value;

		public static readonly VkPointerSize Zero = new(0);

		public VkPointerSize(UIntPtr value) => this.value = value;

		private unsafe VkPointerSize(void* size) => value = new UIntPtr(size);

		public VkPointerSize(uint value) => this.value = new UIntPtr(value);

		public VkPointerSize(ulong value) => this.value = new UIntPtr(value);

		public static bool operator ==(VkPointerSize left, VkPointerSize right) => left.Equals(right);

		public static bool operator !=(VkPointerSize left, VkPointerSize right) => !left.Equals(right);

		public override int GetHashCode() => value.GetHashCode();

		public override string ToString() => value.ToString();

		public override bool Equals(object obj) => obj is VkPointerSize other && Equals(other);

		public bool Equals(VkPointerSize other) => value == other.value;

		public static implicit operator uint(VkPointerSize value) => value.value.ToUInt32();

		public static implicit operator ulong(VkPointerSize value) => value.value.ToUInt64();

		public static implicit operator VkPointerSize(uint value) => new(value);

		public static implicit operator VkPointerSize(ulong value) => new(value);

		public static implicit operator VkPointerSize(UIntPtr value) => new(value);

		public static implicit operator UIntPtr(VkPointerSize value) => value.value;

		public static unsafe implicit operator VkPointerSize(void* value) => new(value);

		public static unsafe implicit operator void*(VkPointerSize value) => (void*)value.value;
	}
}