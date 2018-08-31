using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using RpcLite.AspNetCore.Service;
using CoreConfig = Microsoft.Extensions.Configuration;

namespace RpcLite.Config
{
	/// <summary>
	/// <para>initialize RpcLite and set Default AppHost to RpcManager</para>
	/// <para>in this mode you can use static helper class ClientFactory to get service client instance</para>
	/// <para>Dependency Injection not enabled by default</para>
	/// </summary>
	public class RpcInitializer
	{
		/// <summary>
		/// initialize with RpcConfig
		/// </summary>
		/// <param name="config"></param>
		public static void Initialize(RpcConfig config)
		{
			RpcManager.Initialize(config);
		}

		/// <summary>
		/// initialize with default config file "rpclite.config.json"
		/// </summary>
		public static void Initialize()
		{
			var config = GetConfiguration(null);
			Initialize(config);
		}

		/// <summary>
		/// initialize from configuration
		/// </summary>
		/// <param name="config"></param>
		public static void Initialize(CoreConfig.IConfiguration config)
		{
			//var rpcConfig = RpcConfigHelper.GetConfig(new CoreConfigurationSection(config));
			var rpcConfig = RpcConfigHelper.GetConfig(config);

			Initialize(rpcConfig);
		}

		/// <summary>
		/// initialize with RpcConfigBuilder
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static void Initialize(Action<RpcConfigBuilder> builder)
		{
			Initialize(RpcConfigBuilder.BuildConfig(builder));
		}

		/// <summary>
		/// get IConfigurationRoot from rpclite.config.json
		/// </summary>
		/// <param name="basePath"></param>
		/// <returns></returns>
		public static IConfigurationRoot GetConfiguration(string basePath)
		{
			var configBuilder = new ConfigurationBuilder();

			if (!string.IsNullOrWhiteSpace(basePath))
				configBuilder.SetBasePath(basePath);

			configBuilder
				.AddJsonFile("rpclite.config.json", true);

			var config = configBuilder.Build();
			return config;
		}

		#region for AspNetCore

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
			var config = GetConfiguration(basePath);
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
			var routers = new RouteBuilder(app);
			Initialize(routers, rpcConfig);
			app.UseRouter(routers.Build());
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

		private static void MapService(List<ServiceConfigItem> serviceServices, IRouteBuilder routers)
		{
			foreach (var path in serviceServices)
			{
				routers.MapRoute(path.Path + "{*RpcLiteServicePath}",
					context => RpcManager.ProcessAsync(new AspNetCoreServerContext(context)));
			}
		}
		#endregion

	}
}
