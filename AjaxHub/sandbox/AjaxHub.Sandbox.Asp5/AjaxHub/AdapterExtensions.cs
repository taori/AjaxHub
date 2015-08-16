using System;
using System.Text;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Framework.DependencyInjection;

namespace AjaxHub.Sandbox.Asp5.AjaxHub
{
	public static class AdapterExtensions
	{
		public static HtmlString RenderAjaxHubFunctions(this IHtmlHelper source)
		{
			var accessor = new HttpContextAccessor();
			var uh = accessor.HttpContext.RequestServices.GetRequiredService<IUrlHelper>();
			var test = uh.Action("About", "Home");

			return new HtmlString(test);
		}
	}
}