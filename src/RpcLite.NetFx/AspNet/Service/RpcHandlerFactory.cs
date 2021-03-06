﻿using System.Web;

namespace RpcLite.AspNet.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcHandlerFactory : IHttpHandlerFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="requestType"></param>
		/// <param name="url"></param>
		/// <param name="pathTranslated"></param>
		/// <returns></returns>
		public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
		{
			return new RpcAsyncHandler();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="handler"></param>
		public void ReleaseHandler(IHttpHandler handler)
		{
		}
	}
}
