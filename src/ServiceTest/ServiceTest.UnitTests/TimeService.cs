using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ServiceTest.UnitTests
{
	public interface ITimeService
	{
		string GetDateTime();
		string GetUtcDateTime();
		Task<DateTime> GetDateTimeAsync();
	}

	public class TimeService : ITimeService
	{
		private readonly EmailService _emailService;
		private readonly SmsService _smsService;
		private readonly ITimeService _timeClient;

		public TimeService(ITimeService timeService)
		{
			_timeClient = timeService;
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

		public string GetUtcDateTime()
		{
			var time = _timeClient.GetDateTime();

			return DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
		}
	}
}