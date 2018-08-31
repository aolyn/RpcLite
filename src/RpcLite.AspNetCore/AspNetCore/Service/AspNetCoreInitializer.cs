using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using RpcLite.Config;

namespace RpcLite.AspNetCore.Service
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
			Initialize(app, (string)null);
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
		/// 
		/// </summary>
		/// <param name="app"></param>
		/// <param name="rpcConfig"></param>
		public static void Initialize(IApplicationBuilder app, RpcConfig rpcConfig)
		{
			var appHost = (AppHost) app?.ApplicationServices.GetService(typeof(AppHost));
			if (appHost != null)
			{
				var routers2 = new RouteBuilder(app);
				MapService(rpcConfig.Service.Services, routers2, appHost);
				var routes3 = routers2.Build();
				app.UseRouter(routes3);
				return;
			}

			RpcManager.Initialize(rpcConfig);
			if (app == null) return;

			var routers = new RouteBuilder(app);
			MapService(rpcConfig.Service.Services, routers);
			var routes1 = routers.Build();
			app.UseRouter(routes1);
		}

		private static void MapService(List<ServiceConfigItem> serviceServices, IRouteBuilder routers, AppHost appHost)
		{
			foreach (var path in serviceServices)
			{
				routers.MapRoute(path.Path + "{*RpcLiteServicePath}",
					context => appHost.ProcessAsync(new AspNetCoreServerContext(context)));
			}
		}

		private static void MapService(List<ServiceConfigItem> serviceServices, IRouteBuilder routers)
		{
			foreach (var path in serviceServices)
			{
				routers.MapRoute(path.Path + "{*RpcLiteServicePath}",
					context => RpcManager.ProcessAsync(new AspNetCoreServerContext(context)));
			}
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
			MapService(rpcConfig.Service.Services, routers);
		}
	}
}
