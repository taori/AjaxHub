
using System.Linq;
using NUnit.Framework;

namespace AjaxHub.V452.Test
{
	[TestFixture]
	public class AjaxHubRegisterTests
	{
		[Test]
		public void MethodDiscoveryTest()
		{
			var result = MethodSignatureAssemblyParser.Discover(typeof(TestClass)).ToArray();
			Assert.That(result.Length, Is.EqualTo(2));
		}

		[Test]
		public void MethodNamesTest()
		{
			var result = MethodSignatureAssemblyParser.Discover(typeof(TestClass)).ToArray();
			var resultDict = result.ToDictionary(d => d.ActionName);

			DiscoveryInformation value;
			Assert.That(resultDict.TryGetValue(nameof(TestClass.Method1), out value) && value.ActionName == nameof(TestClass.Method1) && value.ControllerName == typeof(TestClass).Name, $"Unable to find {nameof(TestClass.Method1)}");
			Assert.That(resultDict.TryGetValue(nameof(TestClass.Method2), out value) && value.ActionName == nameof(TestClass.Method2) && value.ControllerName == typeof(TestClass).Name, $"Unable to find {nameof(TestClass.Method2)}");
		}

		public class TestClass
		{
			public void MethodNoAttribute(string a, int b)
			{
			}

			[AjaxHubMethod]
			public void Method1(string a, int b)
			{
			}

			[AjaxHubMethod]
			public void Method2(string a, int b, object c)
			{
			}
		}
	}
}
