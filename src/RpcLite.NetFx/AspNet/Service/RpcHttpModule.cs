using System;
using System.Globalization;
using System.Web;
using RpcLite.Config;

namespace RpcLite.AspNet.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcHttpModule : IHttpModule
	{
		private readonly IHttpHandlerFactory _factory = new RpcHandlerFactory();

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			//clean-up code here.
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context)
		{
			RpcInitializer.Initialize();
			context.PostResolveRequestCache += Context_PostResolveRequestCache;
		}

		private void Context_PostResolveRequestCache(object sender, EventArgs e)
		{
			if (HttpContext.Current?.Request == null) return;
			if (RpcManager.AppHost?.Config?.Service?.Services?.Count == 0) return;

			foreach (var service in RpcManager.AppHost?.Config?.Service?.Services)
			{
				var item = service.Path;

				var start = HttpContext.Current?.Request.ApplicationPath?.Length > 1
					? HttpContext.Current.Request.ApplicationPath.Length + 1
					: 1;
				if (HttpContext.Current.Request.Path.Length - start < item.Length)
					continue;

				var result = CultureInfo.InvariantCulture.CompareInfo.Compare(
					HttpContext.Current.Request.Path, start, item.Length,
					item, 0, item.Length,
					CompareOptions.OrdinalIgnoreCase);
				if (result == 0)
				{
					HttpContext.Current.RemapHandler(_factory.GetHandler(HttpContext.Current, null, null, null));
				}
			}
		}

	}
}
