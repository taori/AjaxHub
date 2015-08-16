namespace AjaxHub.v460.Test.TestSources
{
	public class TestClassNonConventional
	{
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubMethod]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubMethod]
		public void Method2(string a, int b, object c)
		{
		}
	}

	public class TestClassEndingController
	{
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubMethod]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubMethod]
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

		[AjaxHubMethod]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubMethod]
		public void Method2(string a, int b, object c)
		{
		}
	}

	public class TestClassActionNamingTest
	{
		public const string Method1RenameValue = "DifferentName";
		public void MethodNoAttribute(string a, int b)
		{
		}

		[AjaxHubMethod(Method1RenameValue)]
		public void Method1(string a, int b)
		{
		}

		[AjaxHubMethod]
		public void Method2(string a, int b, object c)
		{
		}
	}

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

		[AjaxHubMethod(MethodParameterNames = "d,e,f")]
		public void Method2(string d, int e, object f)
		{
		}
	}
}