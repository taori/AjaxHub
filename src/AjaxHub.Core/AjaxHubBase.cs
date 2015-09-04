using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AjaxAction
{
	public abstract class AjaxHubBase 
	{
		protected virtual ISignatureScanner CreateScanner()
		{
			return new SignatureScannerBase();
		}

		protected abstract IUrlResolver CreateUrlResolver();

		private IUrlResolver _urlResolver;
		protected IUrlResolver GetUrlResolver()
		{
			return _urlResolver ?? (_urlResolver = this.CreateUrlResolver());
		}

		private static readonly List<Assembly> RegisteredAssemblies = new List<Assembly>();

		public static void Register(Assembly assembly)
		{
			RegisteredAssemblies.Add(assembly);
		}

		public static void ResetHubMethodCache()
		{
			_renderedHubMethods = null;
			// todo caching which responds to changes of live sources in asp5 + roslyn (ICompileModule?)
		}

		private static string _renderedHubMethods;
		public string RenderHub()
		{
			if (_renderedHubMethods != null)
				return _renderedHubMethods;

			var writer = new StringWriter();
			writer.Write(RenderHubFunctions(GetAssemblySignatures()));

			return _renderedHubMethods = writer.ToString();
		}

		protected IEnumerable<MethodSignature> GetAssemblySignatures()
		{
			var scanner = CreateScanner();
			return RegisteredAssemblies.SelectMany(assembly => scanner.Scan(assembly));
		}

		internal string RenderHubFunctions(IEnumerable<MethodSignature> signatures)
		{
			// todo maybe a build talk can persist this into a javascript/d.ts file so IDE delivers auto completion
			var writer = new StringWriter();
			var signaturesGrouped = signatures.GroupBy(d => d.ControllerName);

			writer.WriteLine("$.extend(AjaxHub, {");
			var controllerCount = signaturesGrouped.Count();
			var controllerIndex = 0;

			foreach (var currentGroup in signaturesGrouped)
			{
				controllerIndex++;

				writer.WriteLine(" '{0}' : {{", currentGroup.Key);

				var currentControllerSignatures = currentGroup.ToList();

				var lastIndex = currentControllerSignatures.Count - 1;

				for (int index = 0; index < currentControllerSignatures.Count; index++)
				{
					var signature = currentControllerSignatures[index];
					var valueCollection = ConvertSignatureToDictionary(signature);
					signature.OnSignatureSerialized(valueCollection, this);
					var serializedSignatureINfo = JsonConvert.SerializeObject(valueCollection);
					var parameterDelegationObject = BuildArgumentDelegationObject(signature);
					var signatureArgumentCount = signature.MethodArgumentNames.Length;

					var jsParamList = AjaxHubUtility.GetJavascriptParameterCallList(signature);
					if (index < lastIndex)
					{
						writer.WriteLine("  '{0}' : function({2}) {{ AjaxHub._Invoker.execute({{'signature' : {1}, 'values' : {3}, 'extraArguments' : Array.prototype.slice.call(arguments, " + signatureArgumentCount + ") }}); }},", signature.ActionName, serializedSignatureINfo, jsParamList, parameterDelegationObject);
					}
					else
					{
						writer.WriteLine("  '{0}' : function({2}) {{ AjaxHub._Invoker.execute({{'signature' : {1}, 'values' : {3}, 'extraArguments' : Array.prototype.slice.call(arguments, " + signatureArgumentCount + ") }}); }}", signature.ActionName, serializedSignatureINfo, jsParamList, parameterDelegationObject);
					}
				}

				if (controllerIndex < controllerCount)
				{
					writer.WriteLine(" },");
				}
				else
				{
					writer.WriteLine(" }");
				}
			}
			writer.WriteLine("});");

			return writer.ToString();
		}

		protected virtual IDictionary<string, object> OnConvertSignatureToDictionary(MethodSignature signature)
		{
			var result = new Dictionary<string, object>();
			var urlResolver = this.GetUrlResolver();
			result.Add("url", urlResolver.Resolve(signature.ControllerName, signature.ActionName, null));
			result.Add("action", signature.ActionName);
			result.Add("controller", signature.ControllerName);
			result.Add("method", signature.HttpVerb.ToString().ToUpperInvariant());
			result.Add("argumentNames", signature.MethodArgumentNames);
			result.Add("callBefore", signature.HubMethodAttribute.CallBefore);
			result.Add("callAfter", signature.HubMethodAttribute.CallAfter);

			return result;
		}

		internal IDictionary<string, object> ConvertSignatureToDictionary(MethodSignature signature)
		{
			return OnConvertSignatureToDictionary(signature);
		}

		protected string BuildArgumentDelegationObject(MethodSignature signature)
		{
			if (signature.MethodArgumentNames == null || signature.MethodArgumentNames.Length == 0)
				return "{}";

			var root = new JObject();
			foreach (var name in signature.MethodArgumentNames)
			{
				root.Add(name, new JRaw(name));
			}

			return root.ToString(Formatting.None);
		}
	}
}