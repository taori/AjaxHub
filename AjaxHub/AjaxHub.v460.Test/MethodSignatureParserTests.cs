
using System.Collections.Generic;
using System.Linq;
using AjaxHub.v460.Test.TestSources;
using NUnit.Framework;

namespace AjaxHub.V452.Test
{
	[TestFixture]
	public class MethodSIgnatureAssemblyParserTests
	{
		[Test]
		public void DiscoverByType()
		{
			var result = MethodSignatureAssemblyParser.Discover(typeof(TestClassNonConventional)).ToArray();
			Assert.That(result.Length, Is.EqualTo(2));
		}
		[Test]
		public void DiscoverByAssembly()
		{
			var result = MethodSignatureAssemblyParser.Discover(typeof(TestClassNonConventional).Assembly).Where(d => d.ControllerName == typeof(TestClassNonConventional).Name).ToArray();
			Assert.That(result.Length, Is.EqualTo(2));
		}

		[Test]
		public void MethodNamesAutomatic()
		{
			var result = MethodSignatureAssemblyParser.Discover(typeof(TestClassNonConventional)).ToArray();
			var resultDict = result.ToDictionary(d => d.ActionName);

			AjaxHubMethodSignature value;
			Assert.That(resultDict.TryGetValue(nameof(TestClassNonConventional.Method1), out value) && value.ActionName == nameof(TestClassNonConventional.Method1) && value.ControllerName == typeof(TestClassNonConventional).Name, $"Unable to find {nameof(TestClassNonConventional.Method1)}");
			Assert.That(resultDict.TryGetValue(nameof(TestClassNonConventional.Method2), out value) && value.ActionName == nameof(TestClassNonConventional.Method2) && value.ControllerName == typeof(TestClassNonConventional).Name, $"Unable to find {nameof(TestClassNonConventional.Method2)}");
		}

		[Test]
		public void MethodNamesByAttribute()
		{
			var result = MethodSignatureAssemblyParser.Discover(typeof(TestClassActionNamingTest)).ToArray();
			var resultDict = result.ToDictionary(d => d.ActionName);

			var methodName1 = TestClassActionNamingTest.Method1RenameValue;
			var methodName2 = nameof(TestClassActionNamingTest.Method2);

			AjaxHubMethodSignature value;
			Assert.That(resultDict.TryGetValue(methodName1, out value) && value.ActionName == methodName1 && value.ControllerName == typeof(TestClassActionNamingTest).Name, $"Unable to find {methodName1}");
			Assert.That(resultDict.TryGetValue(methodName2, out value) && value.ActionName == methodName2 && value.ControllerName == typeof(TestClassActionNamingTest).Name, $"Unable to find {methodName2}");
		}

		[Test]
		public void ControllerNameByAttribute()
		{
			var testedType = typeof (TestClassWithControllerAttribute);
			var result = MethodSignatureAssemblyParser.Discover(testedType).ToArray();
			var expectation = TestClassWithControllerAttribute.ControllerName;

			var results = new HashSet<string>(result.Select(s => s.ControllerName));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results.First(), Is.EqualTo(expectation));
		}

		[Test]
		public void ControllerNameByClassConvention()
		{
			var testedType = typeof (TestClassEndingController);
			var result = MethodSignatureAssemblyParser.Discover(testedType).ToArray();
			var expectation = typeof(TestClassEndingController).Name.Substring(0, typeof(TestClassEndingController).Name.Length - 10);

			var results = new HashSet<string>(result.Select(s => s.ControllerName));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results.First(), Is.EqualTo(expectation));
		}

		[Test]
		public void ControllerNameByClassUnconventional()
		{
			var testedType = typeof (TestClassNonConventional);
			var result = MethodSignatureAssemblyParser.Discover(testedType).ToArray();
			var expectation = typeof(TestClassNonConventional).Name;

			var results = new HashSet<string>(result.Select(s => s.ControllerName));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results.First(), Is.EqualTo(expectation));
		}

		[Test]
		public void MethodArgumentNames()
		{
			var testedType = typeof (TestClassArgumentNames);
			var result = MethodSignatureAssemblyParser.Discover(testedType).ToArray();

			var method1 = result.FirstOrDefault(d => d.ActionName == TestClassArgumentNames.Method1RenameValue);
			Assert.That(method1, Is.Not.Null);
			var method2 = result.FirstOrDefault(d => d.ActionName == nameof(TestClassArgumentNames.Method2));
			Assert.That(method2, Is.Not.Null);

			Assert.That(method1.ArgumentNames.Length, Is.EqualTo(2));
			Assert.That(method1.ArgumentNames[0], Is.EqualTo("a"));
			Assert.That(method1.ArgumentNames[1], Is.EqualTo("b"));

			Assert.That(method2.ArgumentNames.Length, Is.EqualTo(3));
			Assert.That(method2.ArgumentNames[0], Is.EqualTo("d"));
			Assert.That(method2.ArgumentNames[1], Is.EqualTo("e"));
			Assert.That(method2.ArgumentNames[2], Is.EqualTo("f"));

//			Assert.That(result.Any(d => d.MethodAttribute.ParsedArgumentNames[0]. d.ActionName == TestClassArgumentNames.Method1RenameValue), Is.EqualTo(true));
		}
	}
}
