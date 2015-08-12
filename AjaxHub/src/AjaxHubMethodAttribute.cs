using System;

namespace AjaxHub
{
	public class AjaxHubMethodAttribute : Attribute
	{
		public string Action { get; set; }
		public string Controller { get; set; }
	}
}