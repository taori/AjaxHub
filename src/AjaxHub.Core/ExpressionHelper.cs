﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace AjaxAction
{
	public static class ExpressionHelper
	{
		public static string GetMethodName<TClassType>(Expression<Action<TClassType>> expression)
		{
			var lambdaExpression = expression as LambdaExpression;
			if (lambdaExpression == null)
				throw new ArgumentException("This method expects a lambda expression");

			var methodExpression = expression.Body as MethodCallExpression;
			if(methodExpression != null)
				return methodExpression.Method.Name;

			throw new ArgumentException("Unable to resolve expression");
		}
	}
}