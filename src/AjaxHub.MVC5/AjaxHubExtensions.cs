using System.Web;
using System.Web.Mvc;

namespace AjaxAction
{
	public static class AjaxHubExtensions
	{
		public static IHtmlString RegisterHub(this HtmlHelper source, bool renderScriptTags = true)
		{
			var hub = new AjaxHub();

			if(renderScriptTags)
			{
				return new MvcHtmlString("<script type='text/javascript'>" + hub.RenderHub() + "</script>");
			}
			else
			{
				return new MvcHtmlString(hub.RenderHub());
			}
		}
	}
}