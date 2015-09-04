using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using AjaxAction.MVC5.Test.Environment;
using AjaxAction.MVC5.Test.TestSources;
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

			Assert.That(dictionary["routeTemplate"], Is.EqualTo("TestMethodA/{a}"));
			Assert.That(dictionary["routeName"], Is.EqualTo("TestMethodA"));
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

			Assert.That(dictionary["routeTemplate"], Is.EqualTo("SomePrefix/TestMethodB/{a}"));
			Assert.That(dictionary["routeName"], Is.EqualTo("TestMethodB"));
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

		[Test]
		public void ExpressionNoRoute()
		{
			var value = AjaxHubExpression.For<NoRouteController>(d => d.TestMethod("asd"));
			Assert.That(value, Is.EqualTo("AjaxHub.NoRoute.TestMethod"));
		}

		[Test]
		public void ExpressionRouteTemplateNoPrefix()
		{
			var value = AjaxHubExpression.For<RouteTemplateNoPrefixController>(d => d.TestMethod("asd"));
			Assert.That(value, Is.EqualTo("AjaxHub.RouteTemplateNoPrefix.TestMethodA"));
		}

		[Test]
		public void ExpressionRouteTemplateWithPrefix()
		{
			var value = AjaxHubExpression.For<RouteTemplateWithPrefixController>(d => d.TestMethod("asd"));
			Assert.That(value, Is.EqualTo("AjaxHub.RouteTemplateWithPrefix.TestMethodB"));
		}

		[Test]
		public void ExpressionNoSuffix()
		{
			var value = AjaxHubExpression.For<NoSuffix>(d => d.TestMethod("asd"));
			Assert.That(value, Is.EqualTo($"AjaxHub.{nameof(NoSuffix)}.TestMethod"));
		}
	}
}
