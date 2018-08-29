using System;
using System.Globalization;
using Microsoft.Extensions.Options;

namespace ServiceTest.WebHost
{
	public class TimeService
	{
		private readonly EmailService _emailService;

		public TimeService(IOptions<EmailService> emailService)
		{
			_emailService = emailService.Value;
		}

		public string GetDateTimeString()
		{
			_emailService?.Send("hello");
			return DateTime.Now.ToString(CultureInfo.InvariantCulture);
		}

		public Product GetProductById(int id)
		{
			return new Product
			{
				Id = id,
				Name = "Test Product Name",
				Price = (decimal)(new Random().NextDouble() * 100),
			};
		}
	}
}