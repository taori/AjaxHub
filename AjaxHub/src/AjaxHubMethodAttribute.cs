using System;
using System.Text.RegularExpressions;

namespace AjaxHub
{
	[AttributeUsage(AttributeTargets.Method)]
	public class AjaxHubMethodAttribute : Attribute
	{
		public AjaxHubMethodAttribute()
		{
		}

		public AjaxHubMethodAttribute(string methodName)
		{
			MethodName = methodName;
		}

		public AjaxHubMethodAttribute(string methodName, string methodParameterNames)
		{
			MethodName = methodName;
			MethodParameterNames = methodParameterNames;
		}

		public string MethodName { get; set; }

		/// <summary>
		/// This parameter requires a list of the argument names of a method signature in a pattern like "a,b,c"
		/// </summary>
		public string MethodParameterNames { get; set; }

		private string[] _parsedArgumentNames;

		public string[] ParsedArgumentNames
		{
			get
			{
				if (_parsedArgumentNames != null)
					return _parsedArgumentNames;

				_parsedArgumentNames = SplitNames(MethodParameterNames);
				return _parsedArgumentNames;
			}
		}

		private static readonly Regex SplitPattern = new Regex("[,;\\.]", RegexOptions.Compiled);

		internal static string[] SplitNames(string arguments)
		{
			return SplitPattern.Split(arguments);
		}
	}
}