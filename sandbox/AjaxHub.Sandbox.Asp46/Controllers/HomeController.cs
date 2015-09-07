using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AjaxAction;
using Microsoft.Ajax.Utilities;

namespace Sandbox.Asp46.Controllers
{
	[RoutePrefix("Home")]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}

		[AjaxHubAction]
		public ActionResult TestMethodA()
		{
			return Content("hi");
		}
		
		public ActionResult TestMethodC()
		{
			return Content("i need to stop using vulgar testmessages<script type='text/javascript'>alert(\"because github is just the wrong place for it.\");</script>");
		}

		[AjaxHubAction(ParameterNames = "a,delay", CallBefore = "callBeforeTestMethodB", CallAfter = "callAfterTestMethodB")]
		[Route("TestMethodB/{a}/{delay:int}")]
		[HttpPost]
		public ActionResult TestMethodB(string a, int delay)
		{
			var response = new AjaxActionResponse();

			var content = response.GetActionContent(ControllerContext, helper => helper.Action("TestMethodC").ToString());
			response.Append("body", content);
			response.AddScript(string.Format("alert('{0}');", a));
			response.AddScript(string.Format("alert('{0}');", delay));

			return response;
		}
	}
}