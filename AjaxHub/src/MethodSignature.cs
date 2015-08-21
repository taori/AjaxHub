using System.Collections.Generic;
using System.Diagnostics;

namespace AjaxAction
{
	[DebuggerDisplay("{ToDebug()}")]
	public class MethodSignature
	{
		public MethodSignature(AjaxHubMethodAttribute methodAttribute, string controllerName, string actionName, string[] argumentNames)
		{
			MethodArgumentNames = argumentNames;
			ActionName = actionName;
			ControllerName = controllerName;
			HttpVerb = HttpVerb.Post;
			MethodAttribute = methodAttribute;
		}

		public string ControllerName { get; set; }
		public string ActionName { get; set; }
		public HttpVerb HttpVerb { get; set; }
		public string RouteSignature { get; set; }
		public string[] MethodArgumentNames { get; private set; }
		public AjaxHubMethodAttribute MethodAttribute { get; private set; }

		public string ToDebug()
		{
			return string.Format("signature {0}.{1}", ControllerName, ActionName);
		}

		public void OnSignatureSerialized(IDictionary<string, object> values, AjaxHubBase hub)
		{
			MethodAttribute.OnSignatureSerialized(values, hub);
		}
	}
}