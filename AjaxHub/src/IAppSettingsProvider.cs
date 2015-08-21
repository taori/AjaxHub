namespace AjaxAction
{
	public interface IAppSettingsProvider
	{
		object Get(string key, object defaultValue);
	}
}