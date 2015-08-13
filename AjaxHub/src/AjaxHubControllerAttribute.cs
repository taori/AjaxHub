using System;

namespace AjaxHub
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AjaxHubControllerAttribute : Attribute
	{
		public string Name { get; set; }

		public AjaxHubControllerAttribute(string name)
		{
			Name = name;
		}
	}
}