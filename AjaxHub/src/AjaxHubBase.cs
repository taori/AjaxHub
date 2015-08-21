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
		protected abstract IAppSettingsProvider CreateAppSettingsProvider();

		private IUrlResolver _urlResolver;
		public virtual IUrlResolver GetUrlResolver()
		{
			return _urlResolver ?? (_urlResolver = this.CreateUrlResolver());
		}

		private IAppSettingsProvider _appSettingsProvider;
		public virtual IAppSettingsProvider GetAppSettingsProvider()
		{
			return _appSettingsProvider ?? (_appSettingsProvider = this.CreateAppSettingsProvider());
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
			writer.Write(RenderHubCaller());
			writer.Write(RenderHubFunctions(GetAssemblySignatures()));

			return _renderedHubMethods = writer.ToString();
		}

		public virtual ISignatureJavascriptSerializer CreateSignatureSerializer()
		{
			return new SignatureJavascriptSerializerBase();
		}

		public virtual string RenderHubCaller()
		{
			return @"
var AjaxHubCallRequestStart = function(s){};
var AjaxHubCallRequestDone = function(s){};
var AjaxHubCallStatistics = { runningRequests : 0};
var AjaxHubRequestContainer;

var AjaxHubInitialized = false;

var AjaxHubCall = function(serializedOptions){

debugger;

	if(!AjaxHubInitialized) {
		AjaxHubRequestContainer = $('<div id=""AjaxHubRequestContainer"" style=""display: none;""><div>');
		$('body').eq(0).append(AjaxHubRequestContainer);

		AjaxHubInitialized = true;
	}

	AjaxHubCallStatistics.runningRequests++;
	AjaxHubCallRequestStart(serializedOptions);

	var $executionTarget = $('<div></div>');
	AjaxHubRequestContainer.append($executionTarget);

	$.ajax({
		'url' : serializedOptions.signature.url,
		'data' : serializedOptions.values,
		'traditional' : true,
		'method' : serializedOptions.signature.method
	}).done(function(data){
		$executionTarget.append(data);
		$executionTarget.remove();
		$executionTarget = null;

		AjaxHubCallStatistics.runningRequests--;
		AjaxHubCallRequestDone(serializedOptions);
	});	
};";
		}

		public IEnumerable<MethodSignature> GetAssemblySignatures()
		{
			var scanner = CreateScanner();
			return RegisteredAssemblies.SelectMany(assembly => scanner.Scan(assembly));
		}

		public string RenderHubFunctions(IEnumerable<MethodSignature> signatures)
		{
			var writer = new StringWriter();
			var serializer = CreateSignatureSerializer();
			var signaturesGrouped = signatures.GroupBy(d => d.ControllerName);
			var appSettingsProvider = GetAppSettingsProvider();

			var ajaxHubJsShortcut = appSettingsProvider.Get("AjaxHubShortcutName", "AjaxHub");

			writer.WriteLine("var {0} = {{", ajaxHubJsShortcut);
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

					var jsParamList = GetJavascriptParameterCallList(signature);
					if (index < lastIndex)
					{
						writer.WriteLine("  '{0}' : function({2}) {{ AjaxHubCall({{'signature' : {1}, 'values' : {3}}}); }},", signature.ActionName, serializedSignatureINfo, jsParamList, parameterDelegationObject);
					}
					else
					{
						writer.WriteLine("  '{0}' : function({2}) {{ AjaxHubCall({{'signature' : {1}, 'values' : {3}}}); }}", signature.ActionName, serializedSignatureINfo, jsParamList, parameterDelegationObject);
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
			writer.WriteLine("};");

			return writer.ToString();
		}

		public virtual string GetJavascriptParameterCallList(MethodSignature signature)
		{
			if(signature.MethodArgumentNames != null && signature.MethodArgumentNames.Length > 0)
				return string.Join(", ", signature.MethodArgumentNames);

			return string.Empty;
		}
	}
}