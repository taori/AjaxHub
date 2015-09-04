using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AjaxAction
{
	public static class AjaxHubExpression
	{
		private static readonly string KeywordController = "CONTROLLER";

		public static string For<TController>(Expression<Action<TController>> expression) where TController : IController
		{
			var lambdaExpression = expression as LambdaExpression;
			if (lambdaExpression == null)
				throw new ArgumentException("This method expects a lambda expression");

			var methodExpression = expression.Body as MethodCallExpression;
			if (methodExpression != null)
				return GetFullName<TController>(methodExpression);

			throw new ArgumentException("Unable to resolve expression");
		}

		private static string GetFullName<TController>(MethodCallExpression methodExpression) where TController : IController
		{
			var controllerName = typeof (TController).Name;
			if (controllerName.ToUpperInvariant().EndsWith(KeywordController))
				controllerName = controllerName.Substring(0, controllerName.Length - KeywordController.Length);

			return string.Format("AjaxHub.{0}.{1}", controllerName, GetMethodName<TController>(methodExpression));
		}

		private static string GetMethodName<TController>(MethodCallExpression methodExpression) where TController : IController
		{
			var attr = methodExpression.Method.GetCustomAttribute<AjaxHubActionAttribute>();
			if (attr != null && !string.IsNullOrEmpty(attr.Name))
			{
				return attr.Name;
			}
			return methodExpression.Method.Name;
		}
	}
}
