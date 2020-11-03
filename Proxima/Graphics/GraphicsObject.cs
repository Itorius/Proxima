namespace Proxima.Graphics
{
	public abstract class GraphicsObject
	{
		// internal static readonly List<WeakReference<GraphicsObject>> refs = new List<WeakReference<GraphicsObject>>();

		// public GraphicsObject() => refs.Add(new WeakReference<GraphicsObject>(this, false));

		protected GraphicsDevice graphicsDevice;

		public GraphicsObject(GraphicsDevice graphicsDevice) => this.graphicsDevice = graphicsDevice;

		// public void SetName(string name)
		// {
		// 	
		// 	var i = new VkDebugUtilsObjectNameInfoEXT
		// 	{
		// 		objectType = VkObjectType.ImageView,
		// 		objectHandle = view.Handle,
		// 		sType = VkStructureType.DebugUtilsObjectNameInfoEXT,
		// 		pObjectName = new VkString("depthbuffer image view")
		// 	};
		// 	Vulkan.vkSetDebugUtilsObjectNameEXT(graphicsDevice.LogicalDevice, &i);
		// }
		
		public abstract void Dispose();
	}
}