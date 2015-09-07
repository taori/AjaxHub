using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace AjaxAction
{
	public abstract class AjaxActionResponseBase : IAjaxActionResponse, IDisposable
	{
		public string ContentType { get; set; }

		public Encoding Encoding { get; protected set; }

		protected string GetFullResponseContent()
		{
			return Writer.ToString();
		}

		protected AjaxActionResponseBase() : this(Encoding.UTF8)
		{
		}

		protected AjaxActionResponseBase(Encoding encoding)
		{
			Encoding = encoding;
			Writer = new EncodableStringWriter(encoding);
		}

		protected abstract string EncodeJavascriptString(string content, bool addDoubleQuotes = false);

		public IResetableStringWriter GetStringWriter()
		{
			return Writer;
		}

		string IAjaxActionResponse.WriteContentContainer(string content)
		{
			var guid = "ahr_" +Guid.NewGuid();
			Writer.Write("<div id='{1}'>{0}</div>", content, guid);

			return guid;
		}

		void IAjaxActionResponse.WriteScriptBlock(string content)
		{
			Writer.Write("<script type='text/javascript'>{0}</script>", content);
		}

		protected readonly IResetableStringWriter Writer;

		protected IAjaxActionResponse Unicast
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Injects content into response container.
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public virtual string AddContentContainer(string content)
		{
			return Unicast.WriteContentContainer(content);
		}

		/// <summary>
		/// Injects script block
		/// </summary>
		/// <param name="script"></param>
		public virtual void AddScript(string script)
		{
			Unicast.WriteScriptBlock(script);
		}

		/// <summary>
		/// Adds an alert and escapes it in javascript fashion automatically.
		/// </summary>
		/// <param name="message"></param>
		public virtual void AddAlert(string message)
		{
			Unicast.WriteScriptBlock(string.Format("alert('{0}');", EncodeJavascriptString(message)));
		}
		
		/// <summary>
		/// Executes passed script after prompting the user with a confirmation message
		/// </summary>
		/// <param name="confirmationMessage"></param>
		/// <param name="script"></param>
		public virtual void AddConfirm(string confirmationMessage, string script)
		{
			Unicast.WriteScriptBlock(string.Format("if(confirm('{0}')){{ {1} }}", EncodeJavascriptString(confirmationMessage), script));
		}

		/// <summary>
		/// Clears the entire response content.
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="content"></param>
		public virtual void Clear(string selector, string content)
		{
			Unicast.GetStringWriter().Clear();
		}

		/// <summary>
		/// See http://api.jquery.com/append/
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="content"></param>
		public virtual void Append(string selector, string content)
		{
			var containerId = Unicast.WriteContentContainer(content);
			Unicast.WriteScriptBlock(string.Format("$('{1}').append($('#{0}').contents());", containerId, selector));
		}

		/// <summary>
		/// Sets the content of the selector element you've passed
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="content"></param>
		public virtual void SetContent(string selector, string content)
		{
			var containerId = Unicast.WriteContentContainer(content);
			Unicast.WriteScriptBlock(string.Format("$('{1}').html($('#{0}').contents());", containerId, selector));
		}

		/// <summary>
		/// See http://api.jquery.com/remove/
		/// </summary>
		/// <param name="selector"></param>
		public virtual void Remove(string selector)
		{
			Unicast.WriteScriptBlock(string.Format("$('{0}').remove();", selector));
		}

		/// <summary>
		/// See http://api.jquery.com/detach/
		/// </summary>
		/// <param name="selector"></param>
		public virtual void Detach(string selector)
		{
			Unicast.WriteScriptBlock(string.Format("$('{0}').detach();", selector));
		}

		/// <summary>
		/// See http://api.jquery.com/prepend/
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="content"></param>
		public virtual void Prepend(string selector, string content)
		{
			var containerId = Unicast.WriteContentContainer(content);
			Unicast.WriteScriptBlock(string.Format("$('{1}').prepend($('#{0}').contents());", containerId, selector));
		}

		/// <summary>
		/// See http://api.jquery.com/after/
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="content"></param>
		public virtual void After(string selector, string content)
		{
			var containerId = Unicast.WriteContentContainer(content);
			Unicast.WriteScriptBlock(string.Format("$('{1}').after($('#{0}').contents());", containerId, selector));
		}

		/// <summary>
		/// See http://api.jquery.com/replaceWith/
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="content"></param>
		public virtual void ReplaceWith(string selector, string content)
		{
			var containerId = Unicast.WriteContentContainer(content);
			Unicast.WriteScriptBlock(string.Format("$('{1}').replaceWith($('#{0}').contents());", containerId, selector));
		}

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.Writer.Dispose();
				}

				disposedValue = true;
			}
		}

		~AjaxActionResponseBase()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}