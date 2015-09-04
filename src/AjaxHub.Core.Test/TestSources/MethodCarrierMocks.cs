using System.Diagnostics.CodeAnalysis;
using AjaxAction;

namespace AjaxHub.v460.Test.TestSources
{
	[AjaxHubController]
	public class TestClassNonConventional
	{
		[ExcludeFromCodeCoverage]
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b")]
		[ExcludeFromCodeCoverage]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b,c")]
		[ExcludeFromCodeCoverage]
		public void Method2(string a, int b, object c)
		{
		}
	}
	
	public class TestClassInvalid
	{
		[ExcludeFromCodeCoverage]
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b")]
		[ExcludeFromCodeCoverage]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b,c")]
		[ExcludeFromCodeCoverage]
		public void Method2(string a, int b, object c)
		{
		}
	}

	public class TestClassEndingController
	{
		public const string ExpectedName = "TestClassEnding";

		[ExcludeFromCodeCoverage]
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b")]
		[ExcludeFromCodeCoverage]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b,c")]
		[ExcludeFromCodeCoverage]
		public void Method2(string a, int b, object c)
		{
		}
	}

	[AjaxHubController(ControllerName)]
	public class TestClassWithControllerAttribute
	{
		public const string ControllerName = "NameOverride";

		[ExcludeFromCodeCoverage]
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "d,e")]
		[ExcludeFromCodeCoverage]
		public void Method1(string d, int e)
		{
		}

		[AjaxHubAction(ParameterNames = "f,g,h")]
		[ExcludeFromCodeCoverage]
		public void Method2(string f, int g, object h)
		{
		}
	}

	[AjaxHubController]
	public class TestClassSignatureThrowNoParameters
	{
		[AjaxHubAction]
		[ExcludeFromCodeCoverage]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction]
		[ExcludeFromCodeCoverage]
		public void Method2(string a, int b, object c)
		{
		}
	}

	[AjaxHubController]
	public class TestClassSignatureThrowTooMany
	{
		[AjaxHubAction(ParameterNames = "a,b,c")]
		[ExcludeFromCodeCoverage]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b,c,d")]
		[ExcludeFromCodeCoverage]
		public void Method2(string a, int b, object c)
		{
		}
	}

	[AjaxHubController]
	public class TestClassSignatureThrowTooFew
	{
		[AjaxHubAction(ParameterNames = "a")]
		[ExcludeFromCodeCoverage]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b")]
		[ExcludeFromCodeCoverage]
		public void Method2(string a, int b, object c)
		{
		}
	}

	[AjaxHubController]
	public class TestClassActionNamingTest
	{
		public const string Method1RenameValue = "DifferentName";
		[ExcludeFromCodeCoverage]
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubAction(Method1RenameValue, ParameterNames = "a,b")]
		[ExcludeFromCodeCoverage]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b,c")]
		[ExcludeFromCodeCoverage]
		public void Method2(string a, int b, object c)
		{
		}
	}

	[AjaxHubController]
	public class TestClassArgumentNames
	{
		public const string Method1RenameValue = "DifferentName";
		[ExcludeFromCodeCoverage]
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubAction(Method1RenameValue, "a,b")]
		[ExcludeFromCodeCoverage]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "d,e,f")]
		[ExcludeFromCodeCoverage]
		public void Method2(string d, int e, object f)
		{
		}
	}
}