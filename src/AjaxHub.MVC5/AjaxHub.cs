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
			else
			{
				values.Add("routeTemplate", null);
				values.Add("routeName", null);
			}
			
			return values;
		}
		
		protected override string GetMethodHttpVerb(MethodSignature signature)
		{
			object attribute = signature.MethodInfo.GetCustomAttribute<HttpPostAttribute>();
			if(attribute != null)
				return HttpVerbs.Post.ToString().ToUpperInvariant();

			attribute = signature.MethodInfo.GetCustomAttribute<HttpDeleteAttribute>();
			if(attribute != null)
				return HttpVerbs.Delete.ToString().ToUpperInvariant();

			attribute = signature.MethodInfo.GetCustomAttribute<HttpPutAttribute>();
			if(attribute != null)
				return HttpVerbs.Put.ToString().ToUpperInvariant();

			attribute = signature.MethodInfo.GetCustomAttribute<HttpPatchAttribute>();
			if(attribute != null)
				return HttpVerbs.Patch.ToString().ToUpperInvariant();

			attribute = signature.MethodInfo.GetCustomAttribute<HttpOptionsAttribute>();
			if(attribute != null)
				return HttpVerbs.Options.ToString().ToUpperInvariant();

			attribute = signature.MethodInfo.GetCustomAttribute<HttpHeadAttribute>();
			if(attribute != null)
				return HttpVerbs.Head.ToString().ToUpperInvariant();

			return HttpVerbs.Get.ToString().ToUpperInvariant();
		}
	}
}