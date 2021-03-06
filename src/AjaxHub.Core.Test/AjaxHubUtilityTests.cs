﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AjaxAction;
using AjaxHub.v460.Test.Environment;
using AjaxHub.v460.Test.TestSources;
using NUnit.Framework;

namespace AjaxHub.v460.Test
{
	[TestFixture]
	public class AjaxHubUtilityTests
	{
		[Test]
		public void RandomTypeMethod()
		{
			var value = AjaxHubUtility.GetMethodName<TestClass>(d => d.TestMethodNoParams());
			Assert.That(value, Is.EqualTo(nameof(TestClass.TestMethodNoParams)));
		}
	}
}
