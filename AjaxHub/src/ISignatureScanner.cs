using System;
using System.Collections.Generic;
using System.Reflection;

namespace AjaxAction
{
	public interface ISignatureScanner
	{
		IEnumerable<MethodSignature> Scan(Type type);
		IEnumerable<MethodSignature> Scan(Assembly assembly);

		bool IsValidController(Type type);
		bool IsValidSignature(MethodSignature signature);
		bool IsSearchMatch(Type type, MethodInfo method);
		MethodSignature Create(Type type, MethodInfo method);
	}
}