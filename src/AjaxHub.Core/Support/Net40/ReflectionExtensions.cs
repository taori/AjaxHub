using System;
using System.Reflection;

namespace AjaxAction
{

#if NET40

	public static class ReflectionExtensions
	{
		public static T GetCustomAttribute<T>(this MemberInfo source) where T : Attribute
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