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

		Guid IAjaxActionResponse.WriteContentContainer(string content)
		{
			var guid = Guid.NewGuid();
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
		
		public virtual void AddAlert(string confirmationMessage, string script)
		{
			Unicast.WriteScriptBlock(string.Format("if(confirm('{0}')){{ {1} }}", EncodeJavascriptString(confirmationMessage), script));
		}

		public virtual void Clear(string selector, string content)
		{
			Unicast.GetStringWriter().Clear();
		}

		public virtual void Append(string selector, string content)
		{
			var containerGuid = Unicast.WriteContentContainer(content);
			Unicast.WriteScriptBlock(string.Format("$('{1}').append($('#{0}').contents());", containerGuid, selector));
		}

		public virtual void SetContent(string selector, string content)
		{
			var containerGuid = Unicast.WriteContentContainer(content);
			Unicast.WriteScriptBlock(string.Format("$('{1}').html($('#{0}').contents());", containerGuid, selector));
		}

		public virtual void Remove(string selector)
		{
			Unicast.WriteScriptBlock(string.Format("$('{0}').remove();", selector));
		}

		public virtual void Detach(string selector)
		{
			Unicast.WriteScriptBlock(string.Format("$('{0}').detach();", selector));
		}

		public virtual void Prepend(string selector, string content)
		{
			var containerGuid = Unicast.WriteContentContainer(content);
			Unicast.WriteScriptBlock(string.Format("$('{1}').prepend($('#{0}').contents());", containerGuid, selector));
		}

		public virtual void After(string selector, string content)
		{
			var containerGuid = Unicast.WriteContentContainer(content);
			Unicast.WriteScriptBlock(string.Format("$('{1}').after($('#{0}').contents());", containerGuid, selector));
		}

		public virtual void ReplaceWith(string selector, string content)
		{
			var containerGuid = Unicast.WriteContentContainer(content);
			Unicast.WriteScriptBlock(string.Format("$('{1}').replaceWith($('#{0}').contents());", containerGuid, selector));
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