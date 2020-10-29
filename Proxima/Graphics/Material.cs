using System.Linq;

namespace Proxima.Graphics
{
	public class MaterialInstance : GraphicsObject
	{
		public MaterialInstance(GraphicsDevice graphicsDevice) : base(graphicsDevice)
		{
		}

		public override void Dispose()
		{
		}
	}

	public class Material : GraphicsObject
	{
		public GraphicsPipeline pipeline;
		public Shader shader;

		public Material(GraphicsDevice graphicsDevice, Shader shader) : base(graphicsDevice)
		{
			this.shader = shader;

			pipeline = new GraphicsPipeline(graphicsDevice, pipeline =>
			{
				foreach (var texture in shader.ReflectionData.Textures)
				{
					if (texture.Type == "sampler1D") pipeline.AddTexture(texture.Binding, Renderer2D.PlaceholderTexture1D);
					else pipeline.AddTexture(texture.Binding, Renderer2D.PlaceholderTexture);
				}

				pipeline.SetShader(shader);
				pipeline.AddVertexBuffer(Renderer2D.VertexBuffer);
				pipeline.AddUniformBuffer<Renderer2D.UniformBufferObject>(0);
				pipeline.AddUniformBuffer<Renderer2D.Data>(1);
			});

			graphicsDevice.OnInvalidate += pipeline.Invalidate;
		}

		public void SetTexture(string name, Texture texture)
		{
			uint id = shader.ReflectionData.Textures.FirstOrDefault(x => x.Name == name).Binding;
			pipeline.textures[id] = texture;
			pipeline.UpdateDescriptorSets();
		}

		public void SetUniformBufferData<T>(string name, T data) where T : unmanaged
		{
			uint id = shader.ReflectionData.UBOs.FirstOrDefault(x => x.Name == name).Binding;
			pipeline.GetBuffer(id).SetData(data);
		}

		public override void Dispose()
		{
			graphicsDevice.OnInvalidate -= pipeline.Invalidate;
			pipeline.Dispose();
		}
	}
}