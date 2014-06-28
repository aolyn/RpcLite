using System;
using Contracts;
using Model;

namespace WebApiClient.DAL
{
	class ProductService : ApiServiceBase, IProduct
	{
		#region IProduct Members

		public ProductService(string url, string apiUrlPath, string contentTypeHeader)
			: base(url, apiUrlPath, contentTypeHeader)
		{
		}

		public int Add(Product product)
		{
			return PostResult<Product, int>(product);
		}

		public void Delete(int id)
		{
			throw new NotImplementedException();
		}

		public Product[] Get()
		{
			return GetResult<Product[]>();
		}

		public Product Get(int id)
		{
			return GetResult<int, Product>(id);
		}

		#endregion

		#region IProduct Members


		public int AddProduct(int id, Product product)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProduct Members


		public Product GetById(int id)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IProduct Members


		public int AddProduct(int id, Product product, int a, int b, int c, int d)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
