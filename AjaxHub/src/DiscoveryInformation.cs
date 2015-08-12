using System;
using System.Reflection;

namespace AjaxHub
{
	public class DiscoveryInformation
	{
		public Type IssuedType { get; set; }
		public MethodInfo MethodInfo { get; set; }
		public AjaxHubMethodAttribute Attribute { get; set; }

		public string ActionName
		{
			get { return Attribute.Action ?? MethodInfo.Name; }
		}

		public string ControllerName
		{
			get { return Attribute.Controller ?? GetControllerName(IssuedType); }
		}

		internal static string GetControllerName(Type type)
		{
			return type.Name;
		}

		public DiscoveryInformation(Type issuedType, MethodInfo methodInfo, AjaxHubMethodAttribute attribute)
		{
			IssuedType = issuedType;
			MethodInfo = methodInfo;
			Attribute = attribute;
		}
	}
}