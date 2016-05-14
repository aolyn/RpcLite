using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
					Name = DateTime.Now.ToString(CultureInfo.InvariantCulture),
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

		public Product[] ThrowExcetpion()
		{
			throw new NullReferenceException("test arg");
		}

		public Product Get(int id)
		{
			return new Product
			{
				Id = id,
				Name = DateTime.Now.ToString(CultureInfo.InvariantCulture)
			};
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
			return id;
		}

		//int _addedProductId = -1;
		public void Add(Product product)
		{
			Products.Add(product);
			//_addedProductId = product.Id;
		}

		public Product AddAndGet(Product product)
		{
			return product;
		}

		public async Task<string> GetHtml(string url)
		{
			//throw new ArgumentException("test ex");
			var client = WebRequest.Create(url);
			//var result = Task.Factory.FromAsync(client.BeginGetResponse, client.EndGetResponse);
			var resp = await client.GetResponseAsync();
			var stream = resp.GetResponseStream();
			var buffer = new byte[(int)resp.ContentLength];

			if (stream != null)
			{
				var result2 = await stream.ReadAsync(buffer, 0, buffer.Length);
				var str = System.Text.Encoding.UTF8.GetString(buffer, 0, result2);
				return str;
			}
			return null;
		}

		public async Task GetVoidHtml(string url)
		{
			var client = WebRequest.Create(url);
			var resp = await client.GetResponseAsync();
			var stream = resp.GetResponseStream();
			var buffer = new byte[(int)resp.ContentLength];

			if (stream != null)
			{
				var result2 = await stream.ReadAsync(buffer, 0, buffer.Length);
				var str = Encoding.UTF8.GetString(buffer, 0, result2);
			}
		}

		#region old async

		//public IAsyncResult BeginAdd(Product product, AsyncCallback cb, object state)
		//{
		//	return new Action<Product>(Add).BeginInvoke(product, cb, state);
		//}

		//public int EndAdd(IAsyncResult ar)
		//{
		//	return _addedProductId;
		//}

		//public IAsyncResult BeginEdit(int id, Product product, AsyncCallback cb, object state)
		//{
		//	editProduct = product;
		//	return new Action<Product>(EditInternal).BeginInvoke(product, cb, state);
		//}

		//Product editProduct;
		//public void EditInternal(Product product)
		//{
		//	Thread.Sleep(3000);
		//	product.Category = "Category: " + DateTime.Now.ToShortTimeString();
		//	product.ListDate = DateTime.Now;
		//}

		//public Product EndEdit(IAsyncResult ar)
		//{
		//	return editProduct;
		//}

		//public IAsyncResult BeginCheck(AsyncCallback cb, object state)
		//{
		//	throw new NotImplementedException();
		//}

		//public void EndCheck(IAsyncResult ar)
		//{
		//	throw new NotImplementedException();
		//}

		#endregion

	}
}