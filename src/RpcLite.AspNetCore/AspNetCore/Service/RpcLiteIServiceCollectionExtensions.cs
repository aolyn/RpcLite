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