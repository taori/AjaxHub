namespace AjaxHub.v460.Test.TestSources
{
	[AjaxHubController]
	public class TestClassNonConventional
	{
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubMethod(ParameterNames = "a,b")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubMethod(ParameterNames = "a,b,c")]
		public void Method2(string a, int b, object c)
		{
		}
	}
	
	public class TestClassInvalid
	{
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubMethod(ParameterNames = "a,b")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubMethod(ParameterNames = "a,b,c")]
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

		[AjaxHubMethod(ParameterNames = "a,b")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubMethod(ParameterNames = "a,b,c")]
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

		[AjaxHubMethod(ParameterNames = "d,e")]
		public void Method1(string d, int e)
		{
		}

		[AjaxHubMethod(ParameterNames = "f,g,h")]
		public void Method2(string f, int g, object h)
		{
		}
	}

	[AjaxHubController]
	public class TestClassSignatureThrowNoParameters
	{
		[AjaxHubMethod]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubMethod]
		public void Method2(string a, int b, object c)
		{
		}
	}

	[AjaxHubController]
	public class TestClassSignatureThrowTooMany
	{
		[AjaxHubMethod(ParameterNames = "a,b,c")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubMethod(ParameterNames = "a,b,c,d")]
		public void Method2(string a, int b, object c)
		{
		}
	}

	[AjaxHubController]
	public class TestClassSignatureThrowTooFew
	{
		[AjaxHubMethod(ParameterNames = "a")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubMethod(ParameterNames = "a,b")]
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

		[AjaxHubMethod(Method1RenameValue, ParameterNames = "a,b")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubMethod(ParameterNames = "a,b,c")]
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

		[AjaxHubMethod(Method1RenameValue, "a,b")]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubMethod(ParameterNames = "d,e,f")]
		public void Method2(string d, int e, object f)
		{
		}
	}
}