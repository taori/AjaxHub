using System.Diagnostics;
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
		protected AjaxHubBase GetHub()
		{
			return MockUtil.OfTypeAjaxHub.Default();
		}

		[Test]
		public void JavascriptArgumentDelegation()
		{
			var ajaxHub = GetHub();
			var scanner = new SignatureScannerBase();
			var signatures = scanner.Scan(typeof (TestClassEndingController)).Concat(scanner.Scan(typeof(TestClassWithControllerAttribute)));

			Assert.That(signatures.Count(), Is.GreaterThan(0));
			var signatureWithArguments = signatures.FirstOrDefault(d => d.MethodArgumentNames.Length > 0);
			var argumentList = ajaxHub.GetJavascriptParameterCallList(signatureWithArguments);

			Assert.That(argumentList, Is.Not.Null);
			Assert.That(argumentList.Length, Is.GreaterThan(0));
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
