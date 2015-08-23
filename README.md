# AjaxHub
This purpose of this project is to provide a setup, where it's possible to develop ajax calls in a fashion similar to xajax (php). In this case the target platform will be ASP MVC however.

While php certainly is not the way to do things clean for the majority of the time, xajax definitely turned doing ajax related development not only into a breeze, but helped reducing weaving code.

I am trying to do the same thing here - providing a straight forward way to call your controller actions from javascript without the necessity to switch between razor syntax and javascript all the time.

As of the current date i will target the latest MVC versions
 - MVC 5.2.3
 - MVC 6 (Beta)

If you require support for older versions there should be little overhead if you use only the AjaxHub.Core, because the core assembly is developed with no dependencies on MVC whatsoever.

Usage:

You have to implement your Controller action as following:

		[AjaxHubAction(ParameterNames = "a,delay")]
		public ActionResult TestMethodB(string a, int delay)
		{
			var response = new AjaxActionResponse();

			var content = response.GetActionContent(ControllerContext, helper => helper.Action("TestMethodC").ToString());
			response.AddScript(string.Format("alert('{0}');", a));
			response.AddScript(string.Format("alert('{0}');", delay));
			response.SetContent("body", content);

			return response;
		}
	
In order to call it you can call it like this:

AjaxHub.ControllerName.TestMethodB("some string", 42)

This enables you to update multiple areas of your layout at once. But not only that. You're now able to have in place client logic. No longer will the logic+verification required to invoke one action reside in the view of another. Instead everything is driven by a controller action which handles potential options a user might want to choose from.

More examples to come
