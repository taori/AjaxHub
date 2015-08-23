using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AjaxAction
{
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
			result.Add("callBefore", signature.MethodAttribute.CallBefore);
			result.Add("callAfter", signature.MethodAttribute.CallAfter);

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
}