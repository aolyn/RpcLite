using System.Threading.Tasks;
using Model;

namespace Contracts
{
	public interface IProductAsync
	{
		#region old async
		//IAsyncResult BeginAdd(Product product, AsyncCallback cb, object state);
		//int EndAdd(IAsyncResult ar);

		//IAsyncResult BeginEdit(int id, Product product, AsyncCallback cb, object state);
		//Product EndEdit(IAsyncResult ar);

		//IAsyncResult BeginCheck(AsyncCallback cb, object state);
		//void EndCheck(IAsyncResult ar);

		#endregion

		int AddProduct(int id, Product product);
		int AddProduct(int id, Product product, int a, int b, int c, int d);
		void Delete(int id);
		Product[] Get();
		Product GetById(int id);
		Product Get(int id);

		//exception test
		Product[] ThrowExcetpion();

		Task<string> GetHtml(string url);
		Task GetVoidHtml(string url);

	}
}
