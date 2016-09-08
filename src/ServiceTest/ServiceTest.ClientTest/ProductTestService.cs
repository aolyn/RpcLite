//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Threading.Tasks;
//using ServiceTest.Contract;

//namespace ServiceTest.ClientTest
//{
//	public class ProductTestService : IProductService
//	{
//		static readonly List<Product> Products = new List<Product>();
//		private static readonly Random Rnd = new Random();

//		static ProductTestService()
//		{
//			for (int i = 0; i < 10; i++)
//			{
//				Products.Add(new Product
//				{
//					Id = i,
//					Name = DateTime.Now.ToString(CultureInfo.InvariantCulture),
//					ListDate = DateTime.Now,
//					Category = "aa",
//					Price = Rnd.Next(1000)
//				});
//			}
//		}

//		public int Add(Product product)
//		{
//			return AddAsync(product).Result;
//		}

//		public Task<int> AddAsync(Product product)
//		{
//			Products.Add(product);
//			return Task.FromResult(product.Id);
//		}

//		public Product GetById(int id)
//		{
//			return GetByIdAsync(id).Result;
//		}

//		public Task<Product> GetByIdAsync(int id)
//		{
//			var product = Products.FirstOrDefault(it => it.Id == id);
//			return Task.FromResult(product);
//		}

//		public void ExceptionTest()
//		{
//			throw new NotImplementedException("test exception 235", new InvalidOperationException("test InvalidOperationException"));
//		}

//		public Task ExceptionTestAsync()
//		{
//			throw new NotImplementedException("test exception");
//		}

//		public int GetCount()
//		{
//			return GetCountAsync().Result;
//		}

//		public Task<int> GetCountAsync()
//		{
//			return Task.FromResult(Products.Count);
//		}

//		public void SetCount(int age)
//		{
//			SetCountAsync(age).Wait();
//		}

//		public Task SetCountAsync(int age)
//		{
//			return Task.FromResult<object>(null);
//		}

//		public Product[] GetAll()
//		{
//			return GetAllAsync().Result;
//		}

//		public Product[] GetPage(int pageIndex, int pageSize)
//		{
//			var products = Enumerable.Range(pageIndex, pageSize)
//				.Select(it => new Product
//				{
//					Id = it,
//					Name = "Test Product Name " + it,
//					Price = Rnd.Next() * 100,
//				})
//				.ToArray();
//			return products;
//		}

//		public Task<Product[]> GetAllAsync()
//		{
//			return Task.FromResult(Products.ToArray());
//		}

//	}

//	public interface IProductTestService
//	{
//		int GetCount();

//		Task<int> GetCountAsync();

//		void SetCount(int age);

//		Task SetCountAsync(int age);

//		Product[] GetAll();

//		Product[] GetPage(int pageIndex, int pageSize);

//		Task<Product[]> GetAllAsync();

//		int Add(Product product);

//		Task<int> AddAsync(Product product);

//		Product GetById(int id);

//		Task<Product> GetByIdAsync(int id);

//		void ExceptionTest();

//		Task ExceptionTestAsync();
//	}

//}