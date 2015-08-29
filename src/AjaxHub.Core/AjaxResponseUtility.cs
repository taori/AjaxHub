using System;
using System.IO;

namespace AjaxAction
{
	public class AjaxResponseUtility
	{
		private readonly StringWriter _writer;

		public AjaxResponseUtility(StringWriter writer)
		{
			_writer = writer;
		}

		public virtual Guid AddContentContainer(string content)
		{
			var guid = Guid.NewGuid();
			_writer.Write("<div id='{1}'>{0}</div>", content, guid);

			return guid;
		}

		public virtual void AddScriptBlock(string content)
		{
			_writer.Write("<script type='text/javascript'>{0}</script>", content);
		}
	}
}