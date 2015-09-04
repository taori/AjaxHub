using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AjaxAction
{
	internal static class AjaxHubUtility
	{
		public static string GetJavascriptParameterCallList(MethodSignature signature)
		{
			if (signature.MethodArgumentNames != null && signature.MethodArgumentNames.Length > 0)
				return string.Join(", ", signature.MethodArgumentNames);

			return string.Empty;
		}
	}
}
