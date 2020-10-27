using System.Collections.Generic;
using System.Linq;
using Vortice.Vulkan;

namespace Proxima.Graphics
{
	public class Material : GraphicsObject
	{
		private GraphicsPipeline pipeline;

		internal Material(GraphicsDevice graphicsDevice, Shader shader) : base(graphicsDevice)
		{
			// foreach (var ubo in shader.ReflectionData.SelectMany(x => x.Value.UBOs))
			// {
			// 	
			// }
			//
			// pipeline = new GraphicsPipeline(graphicsDevice, pipeline =>
			// {
			// 	pipeline.SetShader(shader);
			// 	
			// 	foreach (KeyValuePair<VkShaderStageFlags,ReflectionData> pair in shader.ReflectionData)
			// 	{
			// 		foreach (var ubo in pair.Value.UBOs)
			// 		{
			// 			int size = pair.Value.Types[ubo.Type].Members.Sum(x => (x.Type == "mat4" ? 64 : 0));
			//
			// 			// pipeline.AddUniformBuffer(new uni);
			// 		}
			// 	}
			// 	
			// 	
			// 	
			// 	
			// });

			graphicsDevice.OnInvalidate += pipeline.Invalidate;
		}

		public override void Dispose()
		{
			pipeline.Dispose();
		}
	}
}