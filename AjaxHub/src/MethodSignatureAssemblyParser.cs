using System;
using System.Collections.Generic;
using System.Reflection;

namespace AjaxHub
{
	public class MethodSignatureAssemblyParser
	{
		public static IEnumerable<DiscoveryInformation> Discover(Assembly source)
		{
			foreach (var type in source.GetTypes())
			{
				foreach (var discoveryInformation in Discover(type))
				{
					yield return discoveryInformation;
				}
			}
		}

		public static IEnumerable<DiscoveryInformation> Discover(Type source)
		{
			var controller = source.GetCustomAttribute<AjaxHubControllerAttribute>();
			foreach (var method in source.GetMethods())
			{
				var attr = method.GetCustomAttribute<AjaxHubMethodAttribute>();
				if (attr != null)
				{
					yield return new DiscoveryInformation(source, method, attr, controller);
				}
			}
		}

	}
}