﻿using System;
using Aolyn.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RpcLite;
using RpcLite.Config;
using RpcLite.Server.Kestrel;
using Xunit;

namespace ServiceTest.UnitTests.SelfHost
{
	public class SelfHostTest
	{
		private string _serverAddress;
		private string _serviceAddress;

		public SelfHostTest()
		{
			_serverAddress = "http://localhost:" + (40000 + new Random().Next(10000)).ToString();
			_serviceAddress = _serverAddress + "/api/service/";
		}

		[Fact]
		public void Test1()
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseRpcLite(config => config.AddService<TimeService>("api/service/"))
				.UseUrls(_serverAddress)
				.Build();
			host.Start();

			InvokeApiTest();
			host.StopAsync().Wait();
		}

		private void InvokeApiTest()
		{
			var appHost = new AppHost(new RpcConfig());
			var client = appHost.ClientFactory.GetInstance<ITimeService>(_serviceAddress);
			var result = client.GetDateTime();
			Assert.NotNull(result);
		}

		[Fact]
		public void HostBuilderTest()
		{
			var server = new ServerBuilder()
				.UseConfig(config => config.AddService<TimeService>("api/service/"))
				.UseUrls(_serverAddress)
				.Build();
			server.Start();
			InvokeApiTest();
			server.StopAsync().Wait();
		}

		//[Fact]
		//public void ConsulRegistryTest()
		//{
		//	var server = new ServerBuilder()
		//		.UseConfig(config => config.AddService<TimeService>("api/service/"))
		//		.Build();
		//	server.Run();
		//}

		[Fact]
		public void IocTest1()
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseRpcLite(config => config.AddService<TimeService>("api/service/"))
				.ConfigureServices(services => services.AddSingleton<EmailService>())
				.UseUrls(_serverAddress)
				.Build();

			host.Start();
			InvokeApiTest();
			host.StopAsync().Wait();
		}

		[Fact]
		public void UseRpcLiteForWebHostBulderTest()
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseRpcLite(config => config
					.AddService<TimeService>("TimeServiceV1", "api/service/")
					.AddService<TimeService>("TimeService", "api/time/"))
				.UseUrls(_serverAddress)
				.Build();

			host.Start();
			InvokeApiTest();
			host.StopAsync().Wait();
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
				.UseUrls(_serverAddress)
				.Build();

			host.Start();
			InvokeApiTest();
			host.StopAsync().Wait();
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
				.UseUrls(_serverAddress)
				.Build();

			host.Start();
			InvokeApiTest();
			host.StopAsync().Wait();
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