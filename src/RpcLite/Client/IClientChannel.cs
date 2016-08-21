using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RpcLite.Net;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IClientChannel
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
		Task<ResponseMessage> SendAsync(string action, Stream content, IDictionary<string, string> headers);
	}
}
