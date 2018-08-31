using System;
using System.Globalization;
using Microsoft.Extensions.Options;
using ServiceTest.Contract;

namespace ServiceTest.WebHost
{
	public class TimeService
	{
		private readonly EmailService _emailService;
		private readonly IProductService _productService;

		public TimeService(IOptions<EmailService> emailService, IProductService productService)
		{
			_emailService = emailService.Value;
			_productService = productService;
		}

		public string GetDateTime()
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