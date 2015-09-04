using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace AjaxAction
{
	[DebuggerDisplay("{ToDebug()}")]
	public class MethodSignature
	{
		public MethodSignature(Type type, MethodInfo method, AjaxHubActionAttribute hubMethodAttribute, string controllerName, string actionName, string[] argumentNames)
		{
			MethodArgumentNames = argumentNames;
			ActionName = actionName;
			ControllerName = controllerName;
			HubMethodAttribute = hubMethodAttribute;
			ControllerType = type;
			MethodInfo = method;
		}

		public Type ControllerType { get; set; }
		public MethodInfo MethodInfo { get; set; }

		public string ControllerName { get; set; }
		public string ActionName { get; set; }
		public string[] MethodArgumentNames { get; private set; }
		public AjaxHubActionAttribute HubMethodAttribute { get; private set; }

		public string ToDebug()
		{
			return string.Format("signature {0}.{1}", ControllerName, ActionName);
		}

		public void OnSignatureSerialized(IDictionary<string, object> values, AjaxHubBase hub)
		{
			HubMethodAttribute.OnSignatureSerialized(values, hub);
		}
	}
}