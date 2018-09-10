using System;
using System.Globalization;
using System.Threading.Tasks;
using Aolyn.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
				.ConfigureServices(services => services.AddAssembly<SelfHostTest>())
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

		public class TimeService
		{
			private readonly EmailService _emailService;
			private readonly SmsService _smsService;

			public TimeService()
			{
			}

			public TimeService(EmailService emailService, IOptions<SmsService> smsService)
			{
				_emailService = emailService;
				_smsService = smsService.Value;
			}

			public string GetDateTime()
			{
				_emailService?.Send("hello");
				_smsService?.Send("hello");

				return DateTime.Now.ToString(CultureInfo.InvariantCulture);
			}

			public Task<DateTime> GetDateTimeAsync()
			{
				return Task.FromResult(DateTime.Now);
			}
		}

		[Service]
		public class EmailService
		{
			public void Send(string message)
			{
			}
		}

		[Service]
		public class SmsService
		{
			public void Send(string message)
			{
			}
		}
	}
}
