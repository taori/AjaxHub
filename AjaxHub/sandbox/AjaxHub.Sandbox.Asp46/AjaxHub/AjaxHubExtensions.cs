using System;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AjaxAction;

namespace Sandbox.Asp46.AjaxHub
{

	public class AjaxActionResponse : AjaxActionResponseBase
	{
		public AjaxActionResponse()
		{
		}

		public AjaxActionResponse(Encoding encoding) : base(encoding)
		{
		}

		public static implicit operator ContentResult(AjaxActionResponse source)
		{
			return new ContentResult()
			{
				Content = source.GetFullResponseContent(),
				ContentEncoding = source.Encoding,
				ContentType = source.ContentType
			};
		}

		public string GetActionContent(ControllerContext controllerContext, Func<HtmlHelper, string> action)
		{
			var helper = new HtmlHelper(new ViewContext(controllerContext, new RazorView(controllerContext, "fake", "fake", false, null), new ViewDataDictionary(), new TempDataDictionary(), new EncodableStringWriter(Encoding)), new ViewPage());
			return action(helper);
		}
	}

	public class AjaxHub : AjaxHubBase
	{
		protected override IUrlResolver CreateUrlResolver()
		{
			return new UrlResolver(new UrlHelper(HttpContext.Current.Request.RequestContext));
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