using System.Collections.Generic;
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

		protected override IDictionary<string, object> OnConvertSignatureToDictionary(MethodSignature signature)
		{
			var values =  base.OnConvertSignatureToDictionary(signature);
			var routeAttribute = signature.MethodInfo.GetCustomAttribute<RouteAttribute>();
			if (routeAttribute != null)
			{
				string mergedRouteTemplate;
				var prefixAttribute = signature.ControllerType.GetCustomAttribute<RoutePrefixAttribute>();
				if (prefixAttribute == null)
				{
					mergedRouteTemplate = routeAttribute.Template;
				}
				else
				{
					mergedRouteTemplate = prefixAttribute.Prefix + "/" + routeAttribute.Template;
				}

				values.Add("routeTemplate", mergedRouteTemplate);
				values.Add("routeName", routeAttribute.Name);
			}
			
			return values;
		}
	}
}