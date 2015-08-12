using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AjaxHub
{
	public class AjaxHubScriptProvider
	{
		static AjaxHubScriptProvider()
		{
			DefaultMethodStubWriter = new MethodStubWriterDefault();
		}

		private static readonly HashSet<Assembly> RegisteredAssemblies = new HashSet<Assembly>();

		public static IMethodStubWriter DefaultMethodStubWriter { get; set; }

		public static void Register(Assembly assembly)
		{
			RegisteredAssemblies.Add(assembly);
		}

		public static void RenderStubs(TextWriter writer)
		{
			var allInfos = RegisteredAssemblies.SelectMany(assembly => MethodSignatureAssemblyParser.Discover((Assembly) assembly));
			DefaultMethodStubWriter.Resolve(writer, allInfos.OrderBy(d => d.ControllerName).ThenBy(d => d.ActionName));
		}
	}
}