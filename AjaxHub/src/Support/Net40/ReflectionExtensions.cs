using System;
using System.Reflection;

namespace AjaxHub
{

#if NET40

	public static class ReflectionExtensions
	{
		public static T GetCustomAttribute<T>(this MethodInfo source) where T : Attribute
		{
			var attributes = source.GetCustomAttributes(typeof (T), true);
			if (attributes.Length == 0)
				return default(T);

			return attributes[0] as T;
		}

		public static T GetCustomAttribute<T>(this Type source) where T : Attribute
		{
			var attributes = source.GetCustomAttributes(typeof (T), true);
			if (attributes.Length == 0)
				return default(T);

			return attributes[0] as T;
		}

	}

#endif
}