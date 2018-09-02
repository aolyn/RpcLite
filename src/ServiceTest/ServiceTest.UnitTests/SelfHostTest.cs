using System;
using System.Globalization;
using System.Threading.Tasks;
using Aolyn.Extensions.DependencyInjection;
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
				.UseConfig(config => config.AddService<TimeService>("api/service/"))
				.Build();
			host.Run();
		}

		[Fact]
		public void IocTest1()
		{
			var host = new HostBuilder()
				.UseConfig(config => config.AddService<TimeService>("api/service/"))
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
			var host = new HostBuilder()
				.UseConfig(config => config.AddService<TimeService>("api/service/"))
				.ConfigureServices(services => services.AddConfigType<SelfHostTest>())
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

			//var host = new HostBuilder()
			//	.UseConfig(config => config.UseService<Service1>("api/service1/"))
			//	.Build();
			//host.Run();
		}

		public class TimeService
		{
			private readonly EmailService _emailService;

			public TimeService()
			{
			}

			public TimeService(EmailService emailService)
			{
				_emailService = emailService;
			}

			public string GetDateTime()
			{
				_emailService?.Send("hello");

				return DateTime.Now.ToString(CultureInfo.InvariantCulture);
			}

			public Task<DateTime> GetDateTimeAsync()
			{
				return Task.FromResult(DateTime.Now);
			}
		}

		public class EmailService
		{
			public void Send(string message)
			{
			}
		}

		[Fact]
		public void StartupBuilder1()
		{
			var startupType = StartupBuilder.Create(
				config => config.AddService<TimeService>(nameof(TimeService), "api/service1/"));

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
