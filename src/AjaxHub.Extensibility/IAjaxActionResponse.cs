using System;
using System.IO;

namespace AjaxAction
{
    public interface IAjaxActionResponse
    {
	    IResetableStringWriter GetStringWriter();
	    Guid WriteContentContainer(string content);
	    void WriteScriptBlock(string content);
    }
}
