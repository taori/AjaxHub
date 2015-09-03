using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AjaxAction
{
	public class AjaxActionResponse : AjaxActionResponseBase
	{
		public AjaxActionResponse()
		{
		}

		public AjaxActionResponse(Encoding encoding) : base(encoding)
		{
		}

		public static implicit operator ContentResult(AjaxActionResponse source)
		{
			return new ContentResult()
			{
				Content = source.GetFullResponseContent(),
				ContentEncoding = source.Encoding,
				ContentType = source.ContentType
			};
		}

		public string GetActionContent(ControllerContext controllerContext, Func<HtmlHelper, string> action)
		{
			var helper = new HtmlHelper(new ViewContext(controllerContext, new RazorView(controllerContext, "fake", "fake", false, null), new ViewDataDictionary(), new TempDataDictionary(), new EncodableStringWriter(Encoding)), new ViewPage());
			return action(helper);
		}

		protected override string EncodeJavascriptString(string content)
		{
			return HttpUtility.JavaScriptStringEncode(content);
		}
	}
}