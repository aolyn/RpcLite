using System;
using System.Globalization;
using System.Threading.Tasks;

namespace RpcLite.TestService
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcLiteTestService : IRpcLiteTestService
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetDateTimeString()
		{
			return DateTime.Now.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public int Add(int a, int b)
		{
			return a + b;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="product"></param>
		/// <returns></returns>
		public int AddProduct(Product product)
		{
			return product?.Id ?? 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Product GetProductById(int id)
		{
			return new Product
			{
				Id = id,
				Name = "Access to RpcLite",
				Price = 50,
			};
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Task<Product> GetProductByIdAsync(int id)
		{
			return Task.Factory.StartNew(() => new Product
			{
				Id = id,
				Name = "Access to RpcLite",
				Price = 50,
			});
		}

	}
}
