using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace ServiceTest.UnitTests
{
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
}