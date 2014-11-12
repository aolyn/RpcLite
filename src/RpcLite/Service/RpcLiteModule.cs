using System;
using System.Web;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcLiteModule : IHttpModule
	{
		#region IHttpModule Members

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context)
		{
			context.PostResolveRequestCache += context_PostResolveRequestCache;
		}

		void context_PostResolveRequestCache(object sender, EventArgs e)
		{
			//			HttpContext.Current.RemapHandler(
		}

		#endregion
	}
}