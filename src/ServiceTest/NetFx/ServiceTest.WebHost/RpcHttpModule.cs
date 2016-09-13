using System;
using System.Web;

namespace ServiceTest.WebHost
{
	public class RpcHttpModule : IHttpModule
	{
		#region IHttpModule Members

		public void Dispose()
		{
			//clean-up code here.
		}

		public void Init(HttpApplication context)
		{
			context.PostMapRequestHandler += Context_PostMapRequestHandler;
			context.PostResolveRequestCache += Context_PostResolveRequestCache;
		}

		private void Context_PostResolveRequestCache(object sender, EventArgs e)
		{

		}

		private void Context_PostMapRequestHandler(object sender, EventArgs e)
		{
			var handler = HttpContext.Current.Handler;
			var httpContext = sender as HttpContext;
		}

		#endregion

	}
}
