using System;
using System.IO;
using System.Text;

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