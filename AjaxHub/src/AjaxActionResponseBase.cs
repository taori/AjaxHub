using System.Text;

namespace AjaxAction
{
	public abstract class AjaxActionResponseBase
	{
		public string ContentType { get; set; }

		public Encoding Encoding { get; set; }

		protected string GetFullResponseContent()
		{
			return Writer.ToString();
		}

		protected virtual AjaxResponseUtility CreateUtility()
		{
			return new AjaxResponseUtility(Writer);
		}

		protected AjaxActionResponseBase() : this(Encoding.UTF8)
		{
		}

		protected AjaxActionResponseBase(Encoding encoding)
		{
			Encoding = encoding;
			Writer = new EncodableStringWriter(encoding);
		}

		protected readonly EncodableStringWriter Writer;

		private AjaxResponseUtility _utility;

		/// <summary>
		/// This property exists to allow ExtensionMethods to manipulate the response
		/// </summary>
		public AjaxResponseUtility Utility
		{
			get { return _utility ?? (_utility = CreateUtility()); }
		}

		public virtual void AddScript(string script)
		{
			Utility.AddScriptBlock(script);
		}

		public virtual void Append(string selector, string content)
		{
			var containerGuid = Utility.AddContentContainer(content);
			Utility.AddScriptBlock(string.Format("$('{1}').append($('#{0}').contents());", containerGuid, selector));
		}

		public virtual void SetContent(string selector, string content)
		{
			var containerGuid = Utility.AddContentContainer(content);
			Utility.AddScriptBlock(string.Format("$('{1}').html($('#{0}').contents());", containerGuid, selector));
		}

		public virtual void Remove(string selector)
		{
			Utility.AddScriptBlock(string.Format("$('{0}').remove();", selector));
		}

		public virtual void Detach(string selector)
		{
			Utility.AddScriptBlock(string.Format("$('{0}').detach();", selector));
		}

		public virtual void Prepend(string selector, string content)
		{
			var containerGuid = Utility.AddContentContainer(content);
			Utility.AddScriptBlock(string.Format("$('{1}').prepend($('#{0}').contents());", containerGuid, selector));
		}

		public virtual void After(string selector, string content)
		{
			var containerGuid = Utility.AddContentContainer(content);
			Utility.AddScriptBlock(string.Format("$('{1}').after($('#{0}').contents());", containerGuid, selector));
		}

		public virtual void ReplaceWith(string selector, string content)
		{
			var containerGuid = Utility.AddContentContainer(content);
			Utility.AddScriptBlock(string.Format("$('{1}').replaceWith($('#{0}').contents());", containerGuid, selector));
		}
	}
}