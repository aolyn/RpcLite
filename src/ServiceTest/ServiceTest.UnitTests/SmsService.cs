using Aolyn.Extensions.DependencyInjection;

namespace ServiceTest.UnitTests
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