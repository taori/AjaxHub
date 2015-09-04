using System.Diagnostics.CodeAnalysis;
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

		[Test]
		public void JavascriptSignatureNoRoute()
		{
			var hub = new AjaxHubProxy();

			var scanner = new SignatureScannerBase();
			var signatures = scanner.Scan(typeof (NoRouteController)).ToList();

			Assert.That(signatures.Count, Is.GreaterThanOrEqualTo(1));
			var dictionary = hub.ConvertSignatureToDictionary(signatures[0]);
			Assert.That(dictionary, Is.Not.Null);
			Assert.That(dictionary.Count, Is.GreaterThanOrEqualTo(1));

			Assert.That(dictionary["routeTemplate"], Is.EqualTo(null));
			Assert.That(dictionary["routeName"], Is.EqualTo(null));
		}
	}

	public class RouteTemplateNoPrefixController : Controller
	{
		[AjaxHubAction(ParameterNames = "a")]
		[Route("TestMethod/{a}", Name="TestName")]
		[ExcludeFromCodeCoverage]
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
		[ExcludeFromCodeCoverage]
		public ActionResult TestMethod(string a)
		{
			return Content("");
		}
	}
	
	public class NoRouteController : Controller
	{
		[AjaxHubAction(ParameterNames = "a")]
		[ExcludeFromCodeCoverage]
		public ActionResult TestMethod(string a)
		{
			return Content("");
		}
	}
}
