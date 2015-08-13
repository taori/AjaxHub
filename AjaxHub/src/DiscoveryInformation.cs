using System;
using System.Reflection;

namespace AjaxHub
{
	public class DiscoveryInformation
	{
		public DiscoveryInformation(Type issuedType, MethodInfo methodInfo, AjaxHubMethodAttribute methodAttribute, AjaxHubControllerAttribute controllerAttribute)
		{
			IssuedType = issuedType;
			MethodInfo = methodInfo;
			MethodAttribute = methodAttribute;
			ControllerAttribute = controllerAttribute;
		}

		public Type IssuedType { get; set; }
		public MethodInfo MethodInfo { get; set; }
		public AjaxHubMethodAttribute MethodAttribute { get; set; }
		public AjaxHubControllerAttribute ControllerAttribute { get; set; }

		public string ActionName
		{
			get { return MethodAttribute != null && MethodAttribute.Name != null ? MethodAttribute.Name : MethodInfo.Name; }
		}

		public string ControllerName
		{
			get { return ControllerAttribute != null ? ControllerAttribute.Name : GetControllerName(IssuedType); }
		}

		internal static string GetControllerName(Type type)
		{
			if (type.Name.EndsWith("Controller"))
				return type.Name.Substring(0, type.Name.Length - 10);
			return type.Name;
		}
	}
}