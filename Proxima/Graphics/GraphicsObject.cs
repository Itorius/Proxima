namespace Proxima.Graphics
{
	public abstract class GraphicsObject
	{
		// internal static readonly List<WeakReference<GraphicsObject>> refs = new List<WeakReference<GraphicsObject>>();

		// public GraphicsObject() => refs.Add(new WeakReference<GraphicsObject>(this, false));

		protected GraphicsDevice graphicsDevice;

		public GraphicsObject(GraphicsDevice graphicsDevice) => this.graphicsDevice = graphicsDevice;

		public abstract void Dispose();
	}
}