using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace AjaxAction
{
	public class UrlResolver : IUrlResolver
	{
		private readonly UrlHelper _urlHelper;

		public UrlResolver(UrlHelper urlHelper)
		{
			this._urlHelper = urlHelper;
		}

		public string Resolve(string controllerName, string actionName, object values)
		{
			var front = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
			var url = _urlHelper.Action(actionName, controllerName, values) ?? string.Empty;
			var merged = front.TrimEnd('/') + "/" + url.TrimStart('/');

			return merged;
		}
	}
}