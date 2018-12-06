using Aolyn.Extensions.DependencyInjection;

namespace ServiceTest.UnitTests.SelfHost
{
	[Service]
	public class SmsService
	{
		public string Name { get; set; }
		public void Send(string message)
		{
		}
	}
}