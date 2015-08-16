using AjaxAction;
using Moq;

namespace AjaxHub.v460.Test.Environment
{
	public static class MockUtil
	{
		public static class OfTypeAjaxHub
		{
			public static AjaxAction.AjaxHub Default()
			{
				var adapter = new Mock<IAjaxHubAdapter>();
				var services = new Mock<AjaxHubServices>();
				var configrationProvider = new Mock<IConfigurationSettingsProvider>();
				var urlResolver = new Mock<IUrlResolver>();

				urlResolver.Setup(d => d.Resolve(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns<string, string, object>((a, b, c) => $"{a}/{b}");
				configrationProvider.Setup(d => d.Get(It.IsAny<string>(), It.IsAny<object>())).Returns<string, object>((a, b) => b );

				services.Setup(d => d.Configuration).Returns(configrationProvider.Object);
				services.Setup(d => d.UrlResolver).Returns(urlResolver.Object);

				adapter.Setup(d => d.CreateScanner()).Returns(new SignatureScannerBase());
				adapter.Setup(d => d.GetHubServices()).Returns(services.Object);
				var hub = new AjaxAction.AjaxHub(adapter.Object);

				return hub;

			}
		}
	}
}