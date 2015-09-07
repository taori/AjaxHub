using System;
using System.IO;

namespace AjaxAction
{
    public interface IAjaxActionResponse
    {
	    IResetableStringWriter GetStringWriter();
	    string WriteContentContainer(string content);
	    void WriteScriptBlock(string content);
    }
}
