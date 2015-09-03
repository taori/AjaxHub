using System;
using System.IO;
using System.Text;

namespace AjaxAction
{
	public class EncodableStringWriter : StringWriter, IResetableStringWriter
	{
		public EncodableStringWriter(Encoding encoding)
		{
			this.Encoding = encoding;
		}

		public override Encoding Encoding { get; }

		public void Clear()
		{
			this.GetStringBuilder().Clear();
		}
	}
}