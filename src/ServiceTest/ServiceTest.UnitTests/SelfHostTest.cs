using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RpcLite;
using RpcLite.Config;
using RpcLite.Server.Kestrel;
using Xunit;

namespace ServiceTest.UnitTests
{
	public class SelfHostTest
	{
		[Fact]
		public void Test1()
		{
			var host = new HostBuilder()
				.UseConfig(config => config.UseService<Service1>("api/service1/"))
				.Build();
			host.Run();
		}

		[Fact]
		public void IocTest1()
		{
			var host = new HostBuilder()
				.UseConfig(config => config.UseService<Service1>("api/service1/"))
				.ConfigureServices(service => service.AddScoped<Service1>())
				.Build();
			host.Run();
		}

		[Fact]
		public void IocTest()
		{
			var config = new RpcConfigBuilder()
				.UseService<Service1>("api/service1/")
				.Build();

			var services = new ServiceCollection()
				.AddSingleton(typeof(RpcConfig), config)
				.AddSingleton(typeof(AppHost));

			var serviceProvider = services.BuildServiceProvider();
			var appHost = serviceProvider.GetService<AppHost>();

			//var host = new HostBuilder()
			//	.UseConfig(config => config.UseService<Service1>("api/service1/"))
			//	.Build();
			//host.Run();
		}

		public class Service1
		{
			public string GetDateTime()
			{
				return DateTime.Now.ToString(CultureInfo.InvariantCulture);
			}

			public Task<DateTime> GetDateTimeAsync()
			{
				return Task.FromResult(DateTime.Now);
			}
		}

		[Fact]
		public void Test2()
		{
			var startupType = StartupBuilder.Create(
				config => config.UseService<Service1>(nameof(Service1), "api/service1/"));

			var host1 = new WebHostBuilder()
				.UseUrls("http://*:5001")
				.UseStartup(startupType)
				.UseKestrel()
				.Build();
			host1.Run();

			var _ = host1.RunAsync();

			var host2 = new WebHostBuilder()
				.UseUrls("http://*:5002")
				.UseKestrel()
				.UseStartup<Startup>()
				.Build();
			host2.Run();
		}

		public class Startup
		{
			public void ConfigureServices(IServiceCollection services)
			{
			}

			public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
			{
				app.Run(async context =>
				{
					await context.Response.WriteAsync("Hello World!");
				});
			}
		}
	}
}
