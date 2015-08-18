using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AjaxAction
{
	public interface ISignatureJavascriptSerializer
	{
		IDictionary<string, object> SerializeCallValues(MethodSignature signature, AjaxHubBase hub);
		string BuildArgumentDelegationObject(MethodSignature signature);
	}

	public class SignatureJavascriptSerializerBase : ISignatureJavascriptSerializer
	{
		public virtual IDictionary<string, object> SerializeCallValues(MethodSignature signature, AjaxHubBase hub)
		{
			var result = new Dictionary<string, object>();
			var urlResolver = hub.GetUrlResolver();
			result.Add("url", urlResolver.Resolve(signature.ControllerName, signature.ActionName, null));
			result.Add("action", signature.ActionName);
			result.Add("controller", signature.ControllerName);
			result.Add("method", signature.HttpVerb.ToString().ToUpperInvariant());
			result.Add("argumentNames", signature.MethodArgumentNames);

			return result;
		}

		public string BuildArgumentDelegationObject(MethodSignature signature)
		{
			if (signature.MethodArgumentNames == null | signature.MethodArgumentNames.Length == 0)
				return "{}";

			var root = new JObject();
			foreach (var name in signature.MethodArgumentNames)
			{
				root.Add(name, new JRaw(name));
			}

			return root.ToString(Formatting.None);
		}
	}
	
	public interface IAppSettingsProvider
	{
		object Get(string key, object defaultValue);
	}

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

	public class EncodableStringWriter : StringWriter
	{
		public EncodableStringWriter(Encoding encoding)
		{
			this.Encoding = encoding;
		}

		public override Encoding Encoding { get; }
	}

	public class AjaxResponseUtility
	{
		private readonly StringWriter _writer;

		public AjaxResponseUtility(StringWriter writer)
		{
			_writer = writer;
		}

		public Guid AddContentContainer(string content)
		{
			var guid = Guid.NewGuid();
			_writer.Write("<div id='{1}'>{0}</div>", content, guid);

			return guid;
		}

		public void AddScriptBlock(string content)
		{
			_writer.Write("<script type='text/javascript'>{0}</script>", content);
		}
	}

	public class AjaxActionResponse
	{
		public string GetFullResponseContent()
		{
			return _writer.ToString();
		}

		protected virtual AjaxResponseUtility CreateUtility()
		{
			return new AjaxResponseUtility(_writer);
		}

		public AjaxActionResponse() : this(Encoding.UTF8)
		{
		}

		public AjaxActionResponse(Encoding encoding)
		{
			_writer = new EncodableStringWriter(encoding);
		}

		protected readonly EncodableStringWriter _writer;

		private AjaxResponseUtility _utility;

		/// <summary>
		/// This property exists to allow ExtensionMethods to manipulate the response
		/// </summary>
		public AjaxResponseUtility Utility
		{
			get { return _utility ?? (_utility = CreateUtility()); }
		}

		public void AddScript(string script)
		{
			Utility.AddScriptBlock(script);
		}

		public void Append(string selector, string content)
		{
			var containerGuid = Utility.AddContentContainer(content);
			Utility.AddScriptBlock(string.Format("$('#{0}').appendTo($('{1}'));", containerGuid, selector));
		}

		public void Remove(string selector)
		{
			Utility.AddScriptBlock(string.Format("$('{0}').remove();", selector));
		}

		public void Prepend(string selector, string content)
		{
			var containerGuid = Utility.AddContentContainer(content);
			Utility.AddScriptBlock(string.Format("$('#{0}').prependTo($('{1}'));", containerGuid, selector));
		}

		public void InsertAfter(string selector, string content)
		{
			var containerGuid = Utility.AddContentContainer(content);
			Utility.AddScriptBlock(string.Format("$('#{0}').insertAfter($('{1}'));", containerGuid, selector));
		}

		public void ReplaceWith(string selector, string content)
		{
			var containerGuid = Utility.AddContentContainer(content);
			Utility.AddScriptBlock(string.Format("$('{1}').replaceWith($('#{0}'));", containerGuid, selector));
		}

		public void ReplaceAll(string selector, string content)
		{
			var containerGuid = Utility.AddContentContainer(content);
			Utility.AddScriptBlock(string.Format("$('#{0}').replaceAll($('{1}'));", containerGuid, selector));
		}
	}

	public interface IUrlResolver
	{
		string Resolve(string controllerName, string actionName, object values);
	}

	public class SignatureScannerException : Exception
	{
		public SignatureScannerException(MethodInfo methodInfo, Type type)
		{
			MethodInfo = methodInfo;
			Type = type;
		}

		public MethodInfo MethodInfo { get; private set; }

		public Type Type { get; private set; }

		public override string Message
		{
			get
			{
				return string.Format("There is a signature mismatch for \"{0}\" - \"{1}\". Make sure you specify a parameterlist in \"{2}\" if your method contains parameters", Type.FullName, MethodInfo.Name, typeof(AjaxHubMethodAttribute).Name);
			}
		}
	}

	public interface ISignatureScanner
	{
		IEnumerable<MethodSignature> Scan(Type type);
		IEnumerable<MethodSignature> Scan(Assembly assembly);

		bool IsValidController(Type type);
        bool IsValidSignature(MethodSignature signature);
		bool IsSearchMatch(Type type, MethodInfo method);
		MethodSignature Create(Type type, MethodInfo method);
	}

	public enum HttpVerb
	{
		Post,
		Get,
		Put,
		Delete
	}

	[DebuggerDisplay("{ToDebug()}")]
	public class MethodSignature
	{
		public MethodSignature(AjaxHubMethodAttribute methodAttribute, string controllerName, string actionName, string[] argumentNames)
		{
			MethodArgumentNames = argumentNames;
			ActionName = actionName;
			ControllerName = controllerName;
			HttpVerb = HttpVerb.Post;
			MethodAttribute = methodAttribute;
		}

		public string ControllerName { get; set; }
		public string ActionName { get; set; }
		public HttpVerb HttpVerb { get; set; }
		public string RouteSignature { get; set; }
		public string[] MethodArgumentNames { get; private set; }
		public AjaxHubMethodAttribute MethodAttribute { get; private set; }

		public string ToDebug()
		{
			return string.Format("signature {0}.{1}", ControllerName, ActionName);
		}

		public void OnSignatureSerialized(IDictionary<string, object> values, AjaxHubBase hub)
		{
			MethodAttribute.OnSignatureSerialized(values, hub);
		}
	}
}