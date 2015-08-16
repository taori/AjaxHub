using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AjaxHub
{
	public interface ISignatureJavascriptSerializer
	{
		IDictionary<string, object> SerializeCallValues(MethodSignature signature, AjaxHubServices services);
		string BuildArgumentDelegationObject(MethodSignature signature);
	}

	public class SignatureJavascriptSerializerBase : ISignatureJavascriptSerializer
	{
		public IDictionary<string, object> SerializeCallValues(MethodSignature signature, AjaxHubServices services)
		{
			var result = new Dictionary<string, object>();
			result.Add("url", services.UrlResolver.Resolve(signature.ControllerName, signature.ActionName, null));
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

	public class AjaxHubServices
	{
		public virtual IUrlResolver UrlResolver { get; set; }
		public virtual IConfigurationSettingsProvider Configuration { get; set; }
	}

	public interface IConfigurationSettingsProvider
	{
		object Get(string key, object defaultValue);
	}

	public interface IAjaxHubAdapter
	{
		AjaxHubServices GetHubServices();
		ISignatureScanner CreateScanner();
	}

    public abstract class AjaxHubAdapterBase : IAjaxHubAdapter
	{
		public abstract AjaxHubServices GetHubServices();

		public virtual ISignatureScanner CreateScanner()
		{
			return new SignatureScannerBase();
		}
	}

	public class AjaxHub 
	{
		public AjaxHub(IAjaxHubAdapter adapter)
		{
			_adapter = adapter;
		}

		private static readonly List<Assembly> RegisteredAssemblies = new List<Assembly>();

		private IAjaxHubAdapter _adapter;

		public static void Register(Assembly assembly)
		{
			RegisteredAssemblies.Add(assembly);
		}

		public IAjaxHubAdapter Adapter
		{
			get
			{
				if(_adapter == null)
					throw new Exception("AjaxHub.Adapter must be set.");

				return _adapter;
			}
			set { _adapter = value; }
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

            if (Adapter.GetHubServices() == null)
				throw new ArgumentException("hubServices", "hubServices");

			var writer = new StringWriter();
			writer.Write(RenderHubCaller());
			writer.Write(RenderHubFunctions(GetAssemblySignatures()));

			return _renderedHubMethods = RenderHubFunctions(GetAssemblySignatures());
		}

		public virtual ISignatureJavascriptSerializer CreateSignatureSerializer()
		{
			return new SignatureJavascriptSerializerBase();
		}

		public virtual string RenderHubCaller()
		{
			return "var AjaxHubCall = function(serializedOptions){console.log(serializedOptions);}";
		}

		public IEnumerable<MethodSignature> GetAssemblySignatures()
		{
			var scanner = GetSignatureScanner();
			return RegisteredAssemblies.SelectMany(assembly => scanner.Scan(assembly));
		}

		public ISignatureScanner GetSignatureScanner()
		{
			return Adapter.CreateScanner();
		}

		public string RenderHubFunctions(IEnumerable<MethodSignature> signatures)
		{
			var writer = new StringWriter();
			var serializer = CreateSignatureSerializer();
			var signaturesGrouped = signatures.GroupBy(d => d.ControllerName);
			var adapterServices = Adapter.GetHubServices();

			var ajaxHubJsShortcut = adapterServices.Configuration.Get("AjaxHubShortcutName", "AjaxHub");

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
					var valueCollection = serializer.SerializeCallValues(signature, adapterServices);
					signature.OnSignatureSerialized(valueCollection, adapterServices);
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

		public void OnSignatureSerialized(IDictionary<string, object> values, AjaxHubServices services)
		{
			MethodAttribute.OnSignatureSerialized(values, services);
		}
	}
}