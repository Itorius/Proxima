using System.Linq;

namespace Proxima.Graphics
{
	public class Material : GraphicsObject
	{
		private GraphicsPipeline pipeline;
		

		internal Material(GraphicsDevice graphicsDevice, Shader shader) : base(graphicsDevice)
		{
			foreach (var ubo in shader.ReflectionData.SelectMany(x => x.Value.UBOs))
			{
				
			}

			pipeline = new GraphicsPipeline(graphicsDevice, new GraphicsPipeline.Options
			{
				Shader = shader
			}, pipeline =>
			{
				foreach (var ubo in shader.ReflectionData.SelectMany(x => x.Value.UBOs))
				{
				// pipeline.AddUniformBuffer();
				}
				
				
			});

			graphicsDevice.OnInvalidate += pipeline.Invalidate;
		}

		public override void Dispose()
		{
			pipeline.Dispose();
		}
	}
}