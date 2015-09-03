using System.Diagnostics;
using System.Linq;
using System.Text;
using AjaxAction;
using AjaxHub.v460.Test.Environment;
using AjaxHub.v460.Test.TestSources;
using NUnit.Framework;

namespace AjaxHub.v460.Test
{
	[TestFixture]
	public class EncodableStringWriterTests
	{
		[Test]
		public void NoInputNoOutput()
		{
			var writer = new EncodableStringWriter(Encoding.UTF8);

			Assert.That(writer.ToString(), Is.EqualTo(string.Empty));
		}

		[Test]
		public void VerifyInput()
		{
			var writer = new EncodableStringWriter(Encoding.UTF8);
			writer.WriteLine("Test");
			Assert.That(writer.ToString(), Is.EqualTo("Test" + writer.NewLine));
		}

		[Test]
		public void VerifyInputChangedNewLine()
		{
			var writer = new EncodableStringWriter(Encoding.UTF8);
			writer.NewLine = "asd";
			writer.WriteLine("Test");
			Assert.That(writer.ToString(), Is.EqualTo("Test" + writer.NewLine));
		}

		[Test]
		public void Clearable()
		{
			var writer = new EncodableStringWriter(Encoding.UTF8);
			writer.WriteLine("Test");
			writer.Clear();
			Assert.That(writer.ToString(), Is.EqualTo(string.Empty));
		}

		[Test]
		public void ClearWriteableAftwards()
		{
			var writer = new EncodableStringWriter(Encoding.UTF8);
			writer.WriteLine("Test");
			writer.Clear();
			writer.WriteLine("Test2");
			Assert.That(writer.ToString(), Is.EqualTo("Test2" + writer.NewLine));
		}
	}
}
