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
		public ActionResult TestMethodB(string[] a, int delay)
		{
			return Content(string.Format("<script type='text/javascript'>alert('{0}');</script>", string.Join(", ", a)));
		}
	}

	public class AjaxActionResponse
	{
		protected AjaxAction.AjaxHub Create()
		{
			return new AjaxAction.AjaxHub(new AjaxHubAdapterMvc5());
		}
    }
}