using System;

namespace AjaxAction
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AjaxHubControllerAttribute : Attribute
	{
		public AjaxHubControllerAttribute()
		{
		}

		public AjaxHubControllerAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}