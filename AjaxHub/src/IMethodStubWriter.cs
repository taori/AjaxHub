using System.Collections.Generic;
using System.IO;

namespace AjaxHub
{
	public interface IMethodStubWriter
	{
		void Resolve(TextWriter writer, IEnumerable<DiscoveryInformation> informations);
	}
}