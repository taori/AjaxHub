using System.Linq;
using System.Web.Mvc;
using AjaxAction.MVC5.Test.Environment;
using NUnit.Framework;

namespace AjaxAction.MVC5.Test
{
	[TestFixture]
	public class MethodSignatureTests
	{
		[Test]
		public void JavascriptSignatureValuesNonPrefixed()
		{
			var hub = new AjaxHubProxy();

			var scanner = new SignatureScannerBase();
			var signatures = scanner.Scan(typeof (RouteTemplateNoPrefixController)).ToList();

			Assert.That(signatures.Count, Is.GreaterThanOrEqualTo(1));
			var dictionary = hub.ConvertSignatureToDictionary(signatures[0]);
			Assert.That(dictionary, Is.Not.Null);
			Assert.That(dictionary.Count, Is.GreaterThanOrEqualTo(1));

			Assert.That(dictionary["routeTemplate"], Is.EqualTo("TestMethod/{a}"));
			Assert.That(dictionary["routeName"], Is.EqualTo("TestName"));
		}

		[Test]
		public void JavascriptSignatureValuesPrefixed()
		{
			var hub = new AjaxHubProxy();

			var scanner = new SignatureScannerBase();
			var signatures = scanner.Scan(typeof (RouteTemplateWithPrefixController)).ToList();

			Assert.That(signatures.Count, Is.GreaterThanOrEqualTo(1));
			var dictionary = hub.ConvertSignatureToDictionary(signatures[0]);
			Assert.That(dictionary, Is.Not.Null);
			Assert.That(dictionary.Count, Is.GreaterThanOrEqualTo(1));

			Assert.That(dictionary["routeTemplate"], Is.EqualTo("SomePrefix/TestMethod/{a}"));
			Assert.That(dictionary["routeName"], Is.EqualTo("TestName"));
		}
	}

	public class RouteTemplateNoPrefixController : Controller
	{
		[AjaxHubAction(ParameterNames = "a")]
		[Route("TestMethod/{a}", Name="TestName")]
		public ActionResult TestMethod(string a)
		{
			return Content("");
		}
	}

	[RoutePrefix("SomePrefix")]
	public class RouteTemplateWithPrefixController : Controller
	{
		[AjaxHubAction(ParameterNames = "a")]
		[Route("TestMethod/{a}", Name="TestName")]
		public ActionResult TestMethod(string a)
		{
			return Content("");
		}
	}
}
