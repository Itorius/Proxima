using System;
using System.Collections.Generic;

namespace Proxima.Graphics
{
	public abstract class GraphicsObject
	{
		internal static readonly List<WeakReference<GraphicsObject>> refs = new List<WeakReference<GraphicsObject>>();

		public GraphicsObject() => refs.Add(new WeakReference<GraphicsObject>(this, false));

		public abstract void Dispose();
	}
}