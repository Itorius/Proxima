using System;

namespace Proxima
{
	// [Conditional("DEBUG")]
	[AttributeUsage(AttributeTargets.Method)]
	public class ProfileAttribute : Attribute
	{
		public bool WriteToConsole;

		public ProfileAttribute(bool writeToConsole = true)
		{
			WriteToConsole = writeToConsole;
		}
	}
}