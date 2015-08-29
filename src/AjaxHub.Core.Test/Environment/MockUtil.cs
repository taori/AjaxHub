using AjaxAction;
using Moq;

namespace AjaxHub.v460.Test.Environment
{
	public static class MockUtil
	{
		public static class OfTypeAjaxHub
		{
			public static AjaxHubBase Default()
			{
				var hub = new Mock<AjaxHubBase>();
				
				var urlResolver = new Mock<IUrlResolver>();
				
				urlResolver.Setup(d => d.Resolve(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>())).Returns<string, string, object>((a, b, c) => $"{a}/{b}");
				
				hub.Setup(d => d.GetUrlResolver()).Returns(urlResolver.Object);
				hub.Setup(d => d.CreateSignatureSerializer()).Returns(new SignatureJavascriptSerializerBase());
				
				return hub.Object;
			}
		}
	}
}