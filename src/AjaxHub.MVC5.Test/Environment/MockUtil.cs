using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace AjaxAction.MVC5.Test.Environment
{
	public static class MockUtil
	{
		public static class OfTypeAjaxHub
		{
			public static AjaxHub Default()
			{
				var hub = new Mock<AjaxHub>();

				var urlResolver = new Mock<IUrlResolver>();

				urlResolver.Setup(d => d.Resolve(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns<string, string, object>((a, b, c) => $"{a}/{b}");

				return hub.Object;
			}
		}
	}

	public class AjaxHubProxy : AjaxHub
	{
		protected override IUrlResolver CreateUrlResolver()
		{
			return new ProxyResolver();
		}

		public class ProxyResolver : IUrlResolver
		{
			public string Resolve(string controllerName, string actionName, object values)
			{
				return string.Empty;
			}
		}
	}
}
