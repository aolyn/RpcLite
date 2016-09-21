using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using RpcLite.Config;
using RpcLite.Service;
using CoreConfig = Microsoft.Extensions.Configuration;

namespace RpcLite.AspNetCore
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcInitializer
	{
		/// <summary>
		/// initialize with default config file "rpclite.config.json"
		/// </summary>
		public static void Initialize()
		{
			Initialize(null, (string)null);
		}

		/// <summary>
		/// initialize from configuration
		/// </summary>
		/// <param name="config"></param>
		public static void Initialize(CoreConfig.IConfiguration config)
		{
			var rpcConfig = RpcConfigHelper.GetConfig(new CoreConfigurationSection(config));
			//RpcLiteConfig.SetInstance(rpcConfig);

			RpcManager.Initialize(rpcConfig);
		}

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
				routers.MapRoute(path, context => RpcManager.ProcessAsync(new AspNetCoreServerContext(context)));
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
			var config = GetConfiguration(null);
			var rpcConfig = RpcConfigHelper.GetConfig(new CoreConfigurationSection(config));
			Initialize(app, rpcConfig);
		}

		/// <summary>
		/// default initialize from rpclite.config.json
		/// </summary>
		/// <param name="routers">used to UseMiddleware, keep null if not need set</param>
		public static void Initialize(IRouteBuilder routers)
		{
			var config = GetConfiguration(null);
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

		///// <summary>
		///// initialize RpcLite with config in specific basePath
		///// </summary>
		///// <param name="basePath"></param>
		//private static void Initilize(string basePath)
		//{
		//	var config = GetConfiguration(basePath);
		//	Initialize(config);
		//}

		private static IConfigurationRoot GetConfiguration(string basePath)
		{
			var configBuilder = new ConfigurationBuilder();

			if (!string.IsNullOrWhiteSpace(basePath))
				configBuilder.SetBasePath(basePath);

			configBuilder
				.AddJsonFile("rpclite.config.json");

			var config = configBuilder.Build();
			return config;
		}
	}
}
