using System;
using System.Globalization;
using System.Linq;

namespace ServiceTest.WebHost
{
	public class TestService
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

		public Product[] GetAll()
		{
			var rnd = new Random();
			var products = Enumerable.Range(1, 10)
				.Select(it => new Product
				{
					Id = it,
					Name = "Test Product Name",
					Price = (decimal)(rnd.NextDouble() * 100),
				})
				.ToArray();
			return products;
		}

		public Product[] GetAll100()
		{
			var rnd = new Random();
			var products = Enumerable.Range(1, 100)
				.Select(it => new Product
				{
					Id = it,
					Name = "Test Product Name",
					Price = (decimal)(rnd.NextDouble() * 100),
				})
				.ToArray();
			return products;
		}

		public Product[] GetAll1000()
		{
			var rnd = new Random();
			var products = Enumerable.Range(1, 1000)
				.Select(it => new Product
				{
					Id = it,
					Name = "Test Product Name",
					Price = (decimal)(rnd.NextDouble() * 100),
				})
				.ToArray();
			return products;
		}

	}
}