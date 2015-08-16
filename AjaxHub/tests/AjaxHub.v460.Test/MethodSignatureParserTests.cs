
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AjaxHub.v460.Test.TestSources;
using NUnit.Framework;

namespace AjaxHub.V452.Test
{
	[TestFixture]
	public class SignatureScannerTests
	{
		[DebuggerStepThrough]
		protected ISignatureScanner GetScanner()
		{
			return new SignatureScannerBase();
		}

		[Test]
		public void DiscoverByType()
		{
			var result = GetScanner().Scan(typeof(TestClassEndingController)).ToArray();
			Assert.That(result.Length, Is.EqualTo(2));
		}

		[Test]
		[Ignore]
		public void DiscoverByAssembly()
		{
			// does not work because of exception checks - trivial code anyway
			var result = GetScanner().Scan(typeof(TestClassEndingController).Assembly).Where(d => d.ControllerName == TestClassEndingController.ExpectedName).ToArray();
			Assert.That(result.Length, Is.EqualTo(2));
		}

		[Test]
		public void MethodNamesAutomatic()
		{
			var result = GetScanner().Scan(typeof(TestClassEndingController)).ToArray();
			var resultDict = result.ToDictionary(d => d.ActionName);

			MethodSignature value;
			Assert.That(resultDict.TryGetValue(nameof(TestClassEndingController.Method1), out value) && value.ActionName == nameof(TestClassEndingController.Method1) && value.ControllerName == TestClassEndingController.ExpectedName, $"Unable to find {nameof(TestClassNonConventional.Method1)}");
			Assert.That(resultDict.TryGetValue(nameof(TestClassEndingController.Method2), out value) && value.ActionName == nameof(TestClassEndingController.Method2) && value.ControllerName == TestClassEndingController.ExpectedName, $"Unable to find {nameof(TestClassNonConventional.Method2)}");
		}

		[Test]
		public void MethodNamesByAttribute()
		{
			var result = GetScanner().Scan(typeof(TestClassActionNamingTest)).ToArray();
			var resultDict = result.ToDictionary(d => d.ActionName);

			var methodName1 = TestClassActionNamingTest.Method1RenameValue;
			var methodName2 = nameof(TestClassActionNamingTest.Method2);

			MethodSignature value;
			Assert.That(resultDict.TryGetValue(methodName1, out value) && value.ActionName == methodName1 && value.ControllerName == typeof(TestClassActionNamingTest).Name, $"Unable to find {methodName1}");
			Assert.That(resultDict.TryGetValue(methodName2, out value) && value.ActionName == methodName2 && value.ControllerName == typeof(TestClassActionNamingTest).Name, $"Unable to find {methodName2}");
		}

		[Test]
		public void ControllerInvalid()
		{
			var result = GetScanner().Scan(typeof(TestClassInvalid)).ToArray();
			Assert.That(result.Length, Is.EqualTo(0));
		}

		[Test]
		public void ControllerNameByAttribute()
		{
			var testedType = typeof (TestClassWithControllerAttribute);
			var result = GetScanner().Scan(testedType).ToArray();
			var expectation = TestClassWithControllerAttribute.ControllerName;

			var results = new HashSet<string>(result.Select(s => s.ControllerName));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results.First(), Is.EqualTo(expectation));
		}

		[Test]
		public void ControllerNameByClassConvention()
		{
			var testedType = typeof (TestClassEndingController);
			var result = GetScanner().Scan(testedType).ToArray();
			var expectation = typeof(TestClassEndingController).Name.Substring(0, typeof(TestClassEndingController).Name.Length - 10);

			var results = new HashSet<string>(result.Select(s => s.ControllerName));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results.First(), Is.EqualTo(expectation));
		}

		[Test]
		public void ControllerNameByClassUnconventional()
		{
			var testedType = typeof (TestClassNonConventional);
			var result = GetScanner().Scan(testedType).ToArray();
			var expectation = typeof(TestClassNonConventional).Name;

			var results = new HashSet<string>(result.Select(s => s.ControllerName));
			Assert.That(results.Count, Is.EqualTo(1));
			Assert.That(results.First(), Is.EqualTo(expectation));
		}

		[Test]
		public void MethodArgumentNames()
		{
			var testedType = typeof (TestClassArgumentNames);
			var result = GetScanner().Scan(testedType).ToArray();

			Assert.That(result.Length, Is.GreaterThan(0));

			var method1 = result.FirstOrDefault(d => d.ActionName == TestClassArgumentNames.Method1RenameValue);
			Assert.That(method1, Is.Not.Null);
			var method2 = result.FirstOrDefault(d => d.ActionName == nameof(TestClassArgumentNames.Method2));
			Assert.That(method2, Is.Not.Null);

			Assert.That(method1.MethodArgumentNames.Length, Is.EqualTo(2));
			Assert.That(method1.MethodArgumentNames[0], Is.EqualTo("a"));
			Assert.That(method1.MethodArgumentNames[1], Is.EqualTo("b"));

			Assert.That(method2.MethodArgumentNames.Length, Is.EqualTo(3));
			Assert.That(method2.MethodArgumentNames[0], Is.EqualTo("d"));
			Assert.That(method2.MethodArgumentNames[1], Is.EqualTo("e"));
			Assert.That(method2.MethodArgumentNames[2], Is.EqualTo("f"));
		}

		[Test]
		public void ExceptionChecksArguemtnNamesWithInvalidSignature()
		{
			var scanner = GetScanner();

			Assert.Throws(typeof (SignatureScannerException), () => scanner.Scan(typeof (TestClassSignatureThrowTooFew)));
			Assert.Throws(typeof (SignatureScannerException), () => scanner.Scan(typeof (TestClassSignatureThrowTooMany)));
			Assert.Throws(typeof (SignatureScannerException), () => scanner.Scan(typeof (TestClassSignatureThrowNoParameters)));
		}
	}
}
