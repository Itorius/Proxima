using System;

namespace Proxima
{
	// [Conditional("DEBUG")]
	[AttributeUsage(AttributeTargets.Method)]
	public class ProfileAttribute : Attribute
	{
	}
}