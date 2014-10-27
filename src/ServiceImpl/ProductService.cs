using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using Model;

namespace ServiceImpl
{
	public class ProductService : IProduct
	{
		static readonly List<Product> Products = new List<Product>();
		static ProductService()
		{
			var rnd = new Random();
			for (int i = 0; i < 10; i++)
			{
				Products.Add(new Product
				{
					Id = i,
					Name = DateTime.Now.ToString(),
					ListDate = DateTime.Now,
					Category = "aa",
					Price = rnd.Next(1000)
				});
			}
		}

		public Product[] Get()
		{
			return Products.ToArray();
		}

		public Product Get(int id)
		{
			return new Product { Id = id, Name = DateTime.Now.ToString() };
		}

		public int Add(Product product)
		{
			Products.Add(product);
			return product.Id;
		}

		public int AddProduct(int id, Product product)
		{
			Products.Add(product);
			return id;
		}

		public void Delete(int id)
		{
			//return new Product() { Id = id, Name = DateTime.Now.ToString() };
		}

		public Product GetById(int id)
		{
			return Products.FirstOrDefault(it => it.Id == id);
		}

		public int AddProduct(int id, Product product, int a, int b, int c, int d)
		{
			throw new NotImplementedException();
		}
	}
}