using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceTest.UnitTests.Basics
{
	public class ProductService : IProductService
	{
		private static readonly List<Product> Products = new List<Product>();
		private static readonly Random Rnd = new Random();
		private static int _num;

		static ProductService()
		{
			for (int i = 0; i < 10; i++)
			{
				Products.Add(new Product
				{
					Id = i,
					Name = DateTime.Now.ToString(CultureInfo.InvariantCulture),
					ListDate = DateTime.Now,
					Category = "aa",
					Price = Rnd.Next(1000)
				});
			}
		}

		public Task<Product[]> GetPageAsync(int pageIndex, int pageSize)
		{
			return Task.FromResult(GetPage(pageIndex, pageSize));
		}

		public int Add(Product product)
		{
			return AddAsync(product).Result;
		}

		public Task<int> AddAsync(Product product)
		{
			Products.Add(product);
			return Task.FromResult(product?.Id ?? -1);
		}

		public Product GetById(int id)
		{
			return GetByIdAsync(id).Result;
		}

		public Task<Product> GetByIdAsync(int id)
		{
			var product = Products.FirstOrDefault(it => it.Id == id);
			return Task.FromResult(product);
		}

		public void ExceptionTest()
		{
			throw new NotImplementedException("test exception 235", new InvalidOperationException("test InvalidOperationException"));
		}

		public Task ExceptionTestAsync()
		{
			throw new NotImplementedException("test exception");
		}

		public int GetNumber()
		{
			return GetNumberAsync().Result;
		}

		public Task<int> GetNumberAsync()
		{
			return Task.FromResult(_num);
		}

		public void SetNumber(int num)
		{
			SetNumberAsync(num).Wait();
		}

		public Task SetNumberAsync(int num)
		{
			_num = num;
			return Task.FromResult<object>(null);
		}

		public Product[] GetAll()
		{
			return GetAllAsync().Result;
		}

		public Product GetInput(Product product)
		{
			return product;
		}

		public Task<Product> GetInputAsync(Product product)
		{
			return Task.FromResult(product);
		}

		public Product[] GetPage(int pageIndex, int pageSize)
		{
			var products = Enumerable.Range(pageIndex, pageSize)
				.Select(it => new Product
				{
					Id = it,
					Name = "Test Product Name ",// + it,
					Price = Rnd.Next() * 100,
				})
				.ToArray();
			return products;
		}

		public Task<Product[]> GetAllAsync()
		{
			return Task.FromResult(Products.ToArray());
		}

		public void ThrowException(Exception ex)
		{
			throw ex;
		}
	}

}