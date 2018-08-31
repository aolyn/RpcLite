using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ServiceTest.UnitTests
{
	public class DiTest
	{
		private static int _times;

		[Fact]
		public void Test()
		{
			IServiceCollection services = new ServiceCollection();
			services.Add(new ServiceDescriptor(typeof(EmailService), sp =>
			{
				_times++;
				return new EmailService();
			}, ServiceLifetime.Singleton));

			var serviceProvider = services.BuildServiceProvider();
			var emailService = serviceProvider.GetService<EmailService>();
			Assert.Equal(2, _times);
		}

		public class EmailService
		{
			public void Send(string message)
			{
			}
		}
	}
}
