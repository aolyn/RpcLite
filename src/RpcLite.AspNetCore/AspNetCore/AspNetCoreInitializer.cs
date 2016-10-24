using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using RpcLite.Config;
using RpcLite.Service;

namespace RpcLite.AspNetCore
{
	/// <summary>
	/// 
	/// </summary>
	internal class AspNetCoreInitializer
	{
		/// <summary>
		/// default initialize from rpclite.config.json
		/// </summary>
		/// <param name="app">used to UseMiddleware, keep null if not need set</param>
		public static void Initialize(IApplicationBuilder app)
		{
			//var jsonFile = "rpclite.config.json";
			Initialize(app, (string)null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="app"></param>
		/// <param name="rpcConfig"></param>
		public static void Initialize(IApplicationBuilder app, RpcConfig rpcConfig)
		{
			RpcManager.Initialize(rpcConfig);

			if (rpcConfig?.Service?.Paths == null) return;
			if (app == null) return;

			var routers = new RouteBuilder(app);

			foreach (var path in rpcConfig.Service.Paths)
			{
				routers.MapRoute(path + "{*RpcLiteServicePath}", context => RpcManager.ProcessAsync(new AspNetCoreServerContext(context)));
			}
			var routes1 = routers.Build();
			app.UseRouter(routes1);
		}

		/// <summary>
		/// <para>initialize with default config file "rpclite.config.json" in specific basePath</para>
		/// </summary>
		/// <param name="app">used to UseMiddleware, keep null if not need set</param>
		/// <param name="basePath">base path to search config file rpclite.config.json</param>
		public static void Initialize(IApplicationBuilder app, string basePath)
		{
			var config = RpcInitializer.GetConfiguration(basePath);
			var rpcConfig = RpcConfigHelper.GetConfig(new CoreConfigurationSection(config));
			Initialize(app, rpcConfig);
		}

		/// <summary>
		/// default initialize from rpclite.config.json
		/// </summary>
		/// <param name="routers">used to UseMiddleware, keep null if not need set</param>
		public static void Initialize(IRouteBuilder routers)
		{
			var config = RpcInitializer.GetConfiguration(null);
			var rpcConfig = RpcConfigHelper.GetConfig(new CoreConfigurationSection(config));

			Initialize(routers, rpcConfig);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="routers"></param>
		/// <param name="rpcConfig"></param>
		public static void Initialize(IRouteBuilder routers, RpcConfig rpcConfig)
		{
			RpcManager.Initialize(rpcConfig);

			if (rpcConfig?.Service?.Paths != null)
			{
				foreach (var path in rpcConfig.Service.Paths)
				{
					routers.MapRoute(path, context => RpcManager.ProcessAsync(new AspNetCoreServerContext(context)));
				}
			}
		}

	}
}
