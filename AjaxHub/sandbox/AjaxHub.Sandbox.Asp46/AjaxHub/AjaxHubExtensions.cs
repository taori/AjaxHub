using System.Configuration;
using System.Web;
using System.Web.Mvc;
using AjaxAction;

namespace Sandbox.Asp46.AjaxHub
{
	public static class AjaxHubExtensions
	{
		public static IHtmlString RegisterHub(this HtmlHelper source, bool renderScriptTags = true)
		{
			var hub = new AjaxAction.AjaxHub(new AjaxHubAdapterMvc5());

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

	public class AjaxHubAdapterMvc5 : AjaxHubAdapterBase
	{
		public override AjaxHubServices GetHubServices()
		{
			var hubServices = new AjaxHubServices();
			hubServices.Configuration = new ConfigurationSettingsProvider();
			hubServices.UrlResolver = new UrlResolver(new UrlHelper(HttpContext.Current.Request.RequestContext));
            return hubServices;
		}
	}

	public class ConfigurationSettingsProvider : IConfigurationSettingsProvider
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