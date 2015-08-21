using System.Collections.Generic;

namespace AjaxAction
{
	public interface ISignatureJavascriptSerializer
	{
		IDictionary<string, object> SerializeCallValues(MethodSignature signature, AjaxHubBase hub);
		string BuildArgumentDelegationObject(MethodSignature signature);
	}
}