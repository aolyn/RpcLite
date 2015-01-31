using System.Web;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcHandler : IHttpHandler
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(HttpContext context)
		{
			RpcAsyncHandler.ProcessRequestInternal(context);
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsReusable
		{
			get
			{
				return true;
			}
		}
	}
}