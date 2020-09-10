using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

namespace AnalyzerTest
{
	public struct TransformComponent
	{
		public readonly Matrix4x4 transform;

		public TransformComponent(Matrix4x4 transform) => this.transform = transform;
	}

	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			TransformComponent transform = AddComponent<TransformComponent>(Matrix4x4.CreateOrthographic(1280, 720, -1f, 1f));
		}

		public T AddComponent<T>(params object[] args) where T : struct
		{
			T component = (T)Activator.CreateInstance(typeof(T), args);
			return component;
		}
	}
}
