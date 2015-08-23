using System.Web;
using System.Web.Mvc;

namespace AjaxAction
{
	public class AjaxHub : AjaxHubBase
	{
		protected override IUrlResolver CreateUrlResolver()
		{
			return new UrlResolver(new UrlHelper(HttpContext.Current.Request.RequestContext));
		}
	}
}