using System;

namespace AjaxHub
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AjaxHubMethodAttribute : Attribute
	{
		public string Name { get; set; }

		public AjaxHubMethodAttribute()
		{
		}

		public AjaxHubMethodAttribute(string name)
		{
			Name = name;
		}
	}
}