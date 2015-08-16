using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AjaxAction
{
	public class SignatureScannerBase : ISignatureScanner
	{
		public IEnumerable<MethodSignature> Scan(Type source)
		{
			if(source == null)
				throw new ArgumentException("source can not be empty");
			
			foreach (var method in source.GetMethods())
			{
				if(IsSearchMatch(source, method))
				{
					var result = Create(source, method);
					if (result == null)
						continue;

					if (!IsValidSignature(result))
					{
						throw new Exception(string.Format("Created signature does not pass sanity checks: {0}", result.ToDebug()));
					}

					if (!HasCorrectParameterList(method, result))
					{
						throw new SignatureScannerException(method, source);
					}
						
					yield return result;
				}
			}
		}

		private bool HasCorrectParameterList(MethodInfo method, MethodSignature methodSignature)
		{
			return method.GetParameters().Length == methodSignature.MethodArgumentNames.Length;
		}


		public IEnumerable<MethodSignature> Scan(Assembly source)
		{
			if (source == null)
				throw new ArgumentException("source can not be empty");

			foreach (var type in source.GetTypes())
			{
				foreach (var result in Scan(type))
				{
					yield return result;
				}
			}
		}

		public bool IsValidSignature(MethodSignature signature)
		{
			return !string.IsNullOrEmpty(signature.ControllerName)
				&& !string.IsNullOrEmpty(signature.ActionName)
				&& signature.MethodArgumentNames != null;
		}

		public bool IsSearchMatch(Type type, MethodInfo method)
		{
			var controllerType = IsValidController(type);
			var hasMethodAttribute = method.GetCustomAttribute<AjaxHubMethodAttribute>() != null;
			return controllerType && hasMethodAttribute;
		}

		public virtual bool IsValidController(Type type)
		{
			return type.Name.EndsWith("Controller") || type.GetCustomAttribute<AjaxHubControllerAttribute>() != null;
		}

		public virtual MethodSignature Create(Type type, MethodInfo method)
		{
			var methodAttribute = GetAttributeOrThrow<AjaxHubMethodAttribute>(method);

			var controllerName = GetControllerName(type);
			var actionName = GetActionName(type, method);
			var argumentNames = GetArgumentNames(method);
			var result = new MethodSignature(methodAttribute, controllerName, actionName, argumentNames);
			return result;
		}

		protected virtual string[] GetArgumentNames(MethodInfo method)
		{
			var attr = GetAttributeOrThrow<AjaxHubMethodAttribute>(method);

			return SplitNames(attr.ParameterNames);
		}

		protected virtual string GetActionName(Type type, MethodInfo method)
		{
			var attr = GetAttributeOrThrow<AjaxHubMethodAttribute>(method);
			
			if (attr.Name != null)
				return attr.Name;

			return method.Name;
		}

		protected virtual string GetControllerName(Type type)
		{
			var attr = GetAttributeOrThrow<AjaxHubControllerAttribute>(type, false);

			if (attr != null && attr.Name != null)
				return attr.Name;

			if (type.Name.EndsWith("Controller"))
				return type.Name.Substring(0, type.Name.Length - 10);

			return type.Name;
		}

		protected  T GetAttributeOrThrow<T>(MemberInfo source, bool doThrow = true) where T : Attribute
		{
			var attr = source.GetCustomAttribute<T>();
			if (attr == null)
			{
				if( doThrow)
					throw new ArgumentException(string.Format("The default implementation of {0} requires the type to be annotated with a {1} attribute.", typeof(SignatureScannerBase).Name, typeof(T).Name));
				
				return null;
			}
			else
			{
				return attr;
			}
		}

		private static readonly Regex SplitPattern = new Regex("[,;\\.]", RegexOptions.Compiled);
		private static string[] _noArguments = new string[0];
		public virtual string[] SplitNames(string arguments)
		{
			if (arguments == null)
				return _noArguments;

            return SplitPattern.Split(arguments);
		}
	}
}