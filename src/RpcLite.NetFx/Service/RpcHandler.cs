#if !NETCORE

using System.Web;

namespace RpcLite.Service
{
	/// <summary>
	/// in mono 3.12 RpcAsyncHandler can't work use Sync Handler
	/// </summary>
	public class RpcHandler : IHttpHandler
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(HttpContext context)
		{
			RpcManager.ProcessAsync(new AspNetServerContext(context)).Wait();
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsReusable { get { return true; } }
	}
}

#endif
