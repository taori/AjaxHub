using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using AjaxAction;
using Sandbox.Asp46.AjaxHub;

namespace Sandbox.Asp46.Controllers
{
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

		[AjaxHubMethod]
		public ActionResult TestMethodA()
		{
			return Content("hi");
		}

		[AjaxHubMethod(ParameterNames = "a,delay")]
		public ActionResult TestMethodB(string a, int delay)
		{
			var response = new AjaxActionResponse();
			var bodyContent = DateTime.Now.ToString();

//			ViewEngineResult result = ViewEngineCollection.FindView(ControllerContext, "Index", null);
//			if (result.View != null)
//			{
//				return result;
//			}
//			ViewResult d;
//			IView v;


			response.Append("body", bodyContent);
			response.AddScript(string.Format("alert('{0}');", a));
			response.AddScript(string.Format("alert('{0}');", delay));
			var content = response.GetFullResponseContent();

			return Content(content);
		}
	}
}