using System;
using System.Collections.Generic;

namespace AjaxAction
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AjaxHubActionAttribute : Attribute
	{
		public AjaxHubActionAttribute()
		{
		}

		public AjaxHubActionAttribute(string name)
		{
			Name = name;
		}

		public AjaxHubActionAttribute(string name, string parameterNames)
		{
			Name = name;
			ParameterNames = parameterNames;
		}

		public string Name { get; set; }

		/// <summary>
		/// This parameter requires a list of the argument names of a method signature in a pattern like "a,b,c"
		/// </summary>
		public string ParameterNames { get; set; }

		public string CallBefore { get; set; }

		public string CallAfter { get; set; }

		public virtual void OnSignatureSerialized(IDictionary<string, object> values, AjaxHubBase hub)
		{
		}
	}
}