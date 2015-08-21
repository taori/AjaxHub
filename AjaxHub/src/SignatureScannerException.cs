using System;
using System.Reflection;

namespace AjaxAction
{
	public class SignatureScannerException : Exception
	{
		public SignatureScannerException(MethodInfo methodInfo, Type type)
		{
			MethodInfo = methodInfo;
			Type = type;
		}

		public MethodInfo MethodInfo { get; private set; }

		public Type Type { get; private set; }

		public override string Message
		{
			get
			{
				return string.Format("There is a signature mismatch for \"{0}\" - \"{1}\". Make sure you specify a parameterlist in \"{2}\" if your method contains parameters", Type.FullName, MethodInfo.Name, typeof(AjaxHubMethodAttribute).Name);
			}
		}
	}
}