using System;
using System.Collections.Generic;
using System.Linq;
using Contracts;
using Model;

namespace ServiceImpl
{
	public class ProductAsyncService : IProductAsync
	{
		static readonly List<Product> Products = new List<Product>();
		static ProductAsyncService()
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

		//public Product[] Get()
		//{
		//	return Products.ToArray();
		//}

		//public Product Get(int id)
		//{
		//	return new Product { Id = id, Name = DateTime.Now.ToString() };
		//}

		//public int Add(Product product)
		//{
		//	Products.Add(product);
		//	return product.Id;
		//}

		//public int AddProduct(int id, Product product)
		//{
		//	Products.Add(product);
		//	return id;
		//}

		//public void Delete(int id)
		//{
		//	//return new Product() { Id = id, Name = DateTime.Now.ToString() };
		//}

		//public Product GetById(int id)
		//{
		//	return Products.FirstOrDefault(it => it.Id == id);
		//}

		//public int AddProduct(int id, Product product, int a, int b, int c, int d)
		//{
		//	throw new NotImplementedException();
		//}

		public IAsyncResult BeginAdd(Product product, AsyncCallback cb, object state)
		{
			return new Action<Product>(Add).BeginInvoke(product, cb, state);
		}

		int _addedProductId = -1;
		public void Add(Product product)
		{
			Products.Add(product);
			_addedProductId = product.Id;
		}

		public int EndAdd(IAsyncResult ar)
		{
			return _addedProductId;
		}

		public IAsyncResult BeginEdit(int id, Product product, AsyncCallback cb, object state)
		{
			editProduct = product;
			return new Action<Product>(Edit).BeginInvoke(product, cb, state);
		}

		Product editProduct;
		public void Edit(Product product)
		{
		}

		public Product EndEdit(IAsyncResult ar)
		{
			return editProduct;
		}


		public IAsyncResult BeginCheck(AsyncCallback cb, object state)
		{
			throw new NotImplementedException();
		}

		public void EndCheck(IAsyncResult ar)
		{
			throw new NotImplementedException();
		}

		public int AddProduct(int id, Product product)
		{
			return id;
		}
	}
}