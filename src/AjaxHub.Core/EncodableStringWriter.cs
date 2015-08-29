using System.IO;
using System.Text;

namespace AjaxAction
{
	public class EncodableStringWriter : StringWriter
	{
		public EncodableStringWriter(Encoding encoding)
		{
			this.Encoding = encoding;
		}

		public override Encoding Encoding { get; }
	}
}