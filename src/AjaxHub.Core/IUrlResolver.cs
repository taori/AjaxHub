namespace AjaxAction
{
	public interface IUrlResolver
	{
		string Resolve(string controllerName, string actionName, object values);
	}
}