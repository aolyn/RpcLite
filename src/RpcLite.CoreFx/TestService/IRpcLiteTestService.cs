using System.Threading.Tasks;

namespace RpcLite.TestService
{
	/// <summary>
	/// 
	/// </summary>
	public interface IRpcLiteTestService
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string GetDateTimeString();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		int Add(int a, int b);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="product"></param>
		/// <returns></returns>
		int AddProduct(Product product);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Product GetProductById(int id);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<Product> GetProductByIdAsync(int id);

	}
}
