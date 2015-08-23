using AjaxAction;

namespace AjaxHub.v460.Test.TestSources
{
	[AjaxHubController]
	public class TestClassNonConventional
	{
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b,c")]
		public void Method2(string a, int b, object c)
		{
		}
	}
	
	public class TestClassInvalid
	{
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b,c")]
		public void Method2(string a, int b, object c)
		{
		}
	}

	public class TestClassEndingController
	{
		public const string ExpectedName = "TestClassEnding";

        public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b,c")]
		public void Method2(string a, int b, object c)
		{
		}
	}

	[AjaxHubController(ControllerName)]
	public class TestClassWithControllerAttribute
	{
		public const string ControllerName = "NameOverride";

        public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "d,e")]
		public void Method1(string d, int e)
		{
		}

		[AjaxHubAction(ParameterNames = "f,g,h")]
		public void Method2(string f, int g, object h)
		{
		}
	}

	[AjaxHubController]
	public class TestClassSignatureThrowNoParameters
	{
		[AjaxHubAction]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction]
		public void Method2(string a, int b, object c)
		{
		}
	}

	[AjaxHubController]
	public class TestClassSignatureThrowTooMany
	{
		[AjaxHubAction(ParameterNames = "a,b,c")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b,c,d")]
		public void Method2(string a, int b, object c)
		{
		}
	}

	[AjaxHubController]
	public class TestClassSignatureThrowTooFew
	{
		[AjaxHubAction(ParameterNames = "a")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b")]
		public void Method2(string a, int b, object c)
		{
		}
	}

	[AjaxHubController]
	public class TestClassActionNamingTest
	{
		public const string Method1RenameValue = "DifferentName";
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubAction(Method1RenameValue, ParameterNames = "a,b")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "a,b,c")]
		public void Method2(string a, int b, object c)
		{
		}
	}

	[AjaxHubController]
	public class TestClassArgumentNames
	{
		public const string Method1RenameValue = "DifferentName";
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubAction(Method1RenameValue, "a,b")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubAction(ParameterNames = "d,e,f")]
		public void Method2(string d, int e, object f)
		{
		}
	}
}