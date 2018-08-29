using System;
using Microsoft.Extensions.DependencyInjection;
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
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IServiceCollection AddRpcLite(this IServiceCollection services, Action<RpcConfigBuilder> builder)
		{
			var builderObj = new RpcConfigBuilder();
			builder(builderObj);
			var config = builderObj.Build();
			services.AddSingleton(typeof(RpcConfig), config);

			RpcInitializer.Initialize(config, services);
			return services;
		}
	}
}