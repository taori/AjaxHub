using System;

namespace AjaxAction
{
	public interface IResetableStringWriter : IDisposable
	{
		void Clear();
		void Write(string content);
		void Write(string content, params object[] args);
		void WriteLine(string content);
		void WriteLine(string content, params object[] args);
		void Close();
		IFormatProvider FormatProvider { get; }
		string NewLine { get; set; }
    }
}