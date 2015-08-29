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
			return _urlHelper.Action(actionName, controllerName, values);
		}
	}
}