using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AjaxAction;

namespace Sandbox.Asp46.AjaxHub
{

	public class AjaxHub : AjaxHubBase
	{
		protected override IUrlResolver CreateUrlResolver()
		{
			return new UrlResolver(new UrlHelper(HttpContext.Current.Request.RequestContext));
		}

		protected override IAppSettingsProvider CreateAppSettingsProvider()
		{
			return new AppSettingsProvider();
		}
	}

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

	public class AppSettingsProvider : IAppSettingsProvider
	{
		public object Get(string key, object defaultValue)
		{
			var value = ConfigurationManager.AppSettings[key];
			return value ?? defaultValue;
		}
	}

	public class UrlResolver : IUrlResolver
	{
		private UrlHelper _urlHelper;

		public UrlResolver(UrlHelper urlHelper)
		{
			this._urlHelper = urlHelper;
		}

		public string Resolve(string controllerName, string actionName, object values)
		{
			return _urlHelper.Action(actionName, controllerName, values);
		}
	}
}