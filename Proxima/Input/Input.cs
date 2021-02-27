using System.Collections.Generic;
using GLFW;

namespace Proxima
{
	public static class Input
	{
		private static List<Keys> pressedKeys;

		internal static void Initialize(NativeWindow window)
		{
			pressedKeys = new List<Keys>();

			window.KeyAction += (sender, args) =>
			{
				if (args.State == InputState.Press) pressedKeys.Add(args.Key);
				else if (args.State == InputState.Release) pressedKeys.Remove(args.Key);
			};
		}

		public static bool IsKeyDown(Keys key) => pressedKeys.Contains(key);
	}
}