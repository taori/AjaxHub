using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AjaxAction.MVC5.Test.TestSources
{
	public class RouteTemplateNoPrefixController : Controller
	{
		[AjaxHubAction(ParameterNames = "a", Name = "TestMethodA")]
		[Route("TestMethodA/{a}", Name = "TestMethodA")]
		public ActionResult TestMethod(string a)
		{
			return Content("");
		}
	}

	[RoutePrefix("SomePrefix")]
	public class RouteTemplateWithPrefixController : Controller
	{
		[AjaxHubAction(ParameterNames = "a", Name = "TestMethodB")]
		[Route("TestMethodB/{a}", Name = "TestMethodB")]
		public ActionResult TestMethod(string a)
		{
			return Content("");
		}
	}

	public class NoRouteController : Controller
	{
		[AjaxHubAction(ParameterNames = "a")]
		public ActionResult TestMethod(string a)
		{
			return Content("");
		}
	}

	public class NoSuffix : Controller
	{
		[AjaxHubAction(ParameterNames = "a")]
		public ActionResult TestMethod(string a)
		{
			return Content("");
		}
	}
}
