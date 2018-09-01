using System;
using Microsoft.Extensions.DependencyInjection;
using RpcLite;
using RpcLite.Config;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
	/// <summary>
	/// 
	/// </summary>
	public static class RpcLiteIServiceCollectionExtensions
	{
		/// <summary>
		/// initialize with default config file "rpclite.config.json" if exist, or else initialize empty AppHost
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddRpcLite(this IServiceCollection services)
		{
			var config = RpcInitializer.GetConfiguration(null);
			var rpcConfig = RpcConfigHelper.GetConfig(config);
			return AddRpcLite(services, rpcConfig);
		}

		/// <summary>
		/// create AppHost and register to container
		/// </summary>
		/// <param name="services"></param>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IServiceCollection AddRpcLite(this IServiceCollection services, Action<RpcConfigBuilder> builder)
		{
			var config = RpcConfigBuilder.BuildConfig(builder);

			return AddRpcLite(services, config);
		}

		/// <summary>
		/// create AppHost and register to container
		/// </summary>
		/// <param name="services"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IServiceCollection AddRpcLite(this IServiceCollection services, RpcConfig config)
		{
			services.AddSingleton(typeof(RpcConfig), config);
			var _ = new AppHost(config, services);
			return services;
		}
	}
}