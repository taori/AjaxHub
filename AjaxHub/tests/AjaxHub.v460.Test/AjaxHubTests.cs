﻿using System.Diagnostics;
using System.Linq;
using AjaxAction;
using AjaxHub.v460.Test.Environment;
using AjaxHub.v460.Test.TestSources;
using NUnit.Framework;

namespace AjaxHub.v460.Test
{
	[TestFixture]
	public class AjaxHubTests
	{
		[DebuggerStepThrough]
		protected AjaxAction.AjaxHubBase GetHub()
		{
			return MockUtil.OfTypeAjaxHub.Default();
		}

		[Test]
		public void SignatureGeneration()
		{
			var ajaxHub = GetHub();
			var scanner = new SignatureScannerBase();
			var signatures = scanner.Scan(typeof (TestClassEndingController)).Concat(scanner.Scan(typeof(TestClassWithControllerAttribute)));
			var result = ajaxHub.RenderHubFunctions(signatures);
			Assert.That(result.Length, Is.GreaterThan(0));
		}
	}
}
