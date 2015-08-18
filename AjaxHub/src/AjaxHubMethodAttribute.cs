using System;
using System.Collections.Generic;

namespace AjaxAction
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AjaxHubMethodAttribute : Attribute
	{
		public AjaxHubMethodAttribute()
		{
		}

		public AjaxHubMethodAttribute(string name)
		{
			Name = name;
		}

		public AjaxHubMethodAttribute(string name, string parameterNames)
		{
			Name = name;
			ParameterNames = parameterNames;
		}

		public string Name { get; set; }

		/// <summary>
		/// This parameter requires a list of the argument names of a method signature in a pattern like "a,b,c"
		/// </summary>
		public string ParameterNames { get; set; }

		public virtual void OnSignatureSerialized(IDictionary<string, object> values, AjaxHubBase hub)
		{
		}
	}
}