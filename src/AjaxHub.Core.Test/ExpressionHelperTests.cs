using System.Diagnostics;
using System.Linq;
using AjaxAction;
using AjaxHub.v460.Test.Environment;
using AjaxHub.v460.Test.TestSources;
using NUnit.Framework;

namespace AjaxHub.v460.Test
{
	[TestFixture]
	public class ExpressionHelperTests
	{
		[Test]
		public void RandomTypeMethod()
		{
			var value = ExpressionHelper.GetMethodName<TestClass>(d => d.TestMethodNoParams());
			Assert.That(value, Is.EqualTo(nameof(TestClass.TestMethodNoParams)));
		}

		public class TestClass
		{
			public void TestMethodNoParams()
			{
			} 
		}
	}
}
