using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

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
		public virtual IUrlResolver GetUrlResolver()
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

		public virtual ISignatureJavascriptSerializer CreateSignatureSerializer()
		{
			return new SignatureJavascriptSerializerBase();
		}

		public IEnumerable<MethodSignature> GetAssemblySignatures()
		{
			var scanner = CreateScanner();
			return RegisteredAssemblies.SelectMany(assembly => scanner.Scan(assembly));
		}

		public string RenderHubFunctions(IEnumerable<MethodSignature> signatures)
		{
			// todo maybe a build talk can persist this into a javascript/d.ts file so IDE delivers auto completion
			var writer = new StringWriter();
			var serializer = CreateSignatureSerializer();
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
					var valueCollection = serializer.SerializeCallValues(signature, this);
					signature.OnSignatureSerialized(valueCollection, this);
					var serializedSignatureINfo = JsonConvert.SerializeObject(valueCollection);
					var parameterDelegationObject = serializer.BuildArgumentDelegationObject(signature);
					var signatureArgumentCount = signature.MethodArgumentNames.Length;

					var jsParamList = GetJavascriptParameterCallList(signature);
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

		public string GetJavascriptParameterCallList(MethodSignature signature)
		{
			if(signature.MethodArgumentNames != null && signature.MethodArgumentNames.Length > 0)
				return string.Join(", ", signature.MethodArgumentNames);

			return string.Empty;
		}
	}
}