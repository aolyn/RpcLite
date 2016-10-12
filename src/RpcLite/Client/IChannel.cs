using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IChannel
	{
		/// <summary>
		/// 
		/// </summary>
		string Address { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="content"></param>
		/// <param name="headers"></param>
		/// <returns></returns>
		Task<IResponseMessage> SendAsync(string action, Stream content, IDictionary<string, string> headers);
	}
}
