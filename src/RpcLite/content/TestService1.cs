using System;
using System.Globalization;

namespace RpcLiteServiceTest
{
	public class TestService1
	{
		public string GetDateTimeString()
		{
			return DateTime.Now.ToString(CultureInfo.InvariantCulture);
		}

		public int AddProduct(Product product)
		{
			return 1;
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

	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
	}

	public interface ITestService
	{
		string GetDateTimeString();
		int AddProduct(Product product);
		Product GetProductById(int id);
	}
}