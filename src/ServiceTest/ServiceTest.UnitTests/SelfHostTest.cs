using System;
using Aolyn.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseRpcLite(config => config.AddService<TimeService>("api/service/"))
				.Build();
			host.Run();
		}

		[Fact]
		public void HostBuilderTest()
		{
			var server = new ServerBuilder()
				.UseConfig(config => config.AddService<TimeService>("api/service/"))
				.Build();
			server.Run();
		}


		[Fact]
		public void ConsulRegistryTest()
		{
			var server = new ServerBuilder()
				.UseConfig(config => config.AddService<TimeService>("api/service/"))
				.Build();
			server.Run();
		}

		[Fact]
		public void IocTest1()
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseRpcLite(config => config.AddService<TimeService>("api/service/"))
				.ConfigureServices(services => services.AddSingleton<EmailService>())
				.Build();
			host.Run();
		}

		[Fact]
		public void UseRpcLiteForWebHostBulderTest()
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseRpcLite(config => config
					.AddService<TimeService>("TimeServiceV1", "api/service/")
					.AddService<TimeService>("TimeService", "api/time/"))
				.Build();
			host.Run();
		}

		[Fact]
		public void IocTest2()
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseRpcLite(config => config
					.AddService<TimeService>("TimeServiceV1", "api/service/")
					.AddService<TimeService>("TimeService", "api/time/"))
				.ConfigureServices(services => services.AddConfigurationAssembly<SelfHostTest>())
				.Build();
			host.Run();
		}

		[Fact]
		public void IocTest3()
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseRpcLite(config => config
					.AddService<TimeService>("TimeServiceV1", "api/service/")
					.AddService<TimeService>("TimeService", "api/time/"))
				.ConfigureServices(services => services
					.AddSingleton<EmailService>()
					//.Configure<SmsService>(opt => { opt.Name = "TestV1"; })
					.AddSingleton(new SmsService
					{
						Name = "from new inject"
					}))
				.Build();
			host.Run();
		}

		[Service(ServiceLifetime.Singleton)]
		public static EmailService GetEmailService(IServiceProvider serviceProvider)
		{
			return new EmailService();
		}

		[Fact]
		public void IocTest()
		{
			var config = new RpcConfigBuilder()
				.AddService<TimeService>("api/service/")
				.Build();

			var services = new ServiceCollection()
				.AddSingleton(typeof(RpcConfig), config)
				.AddSingleton(typeof(AppHost));

			var serviceProvider = services.BuildServiceProvider();
			var appHost = serviceProvider.GetService<AppHost>();
			Assert.NotNull(appHost);
		}
	}
}