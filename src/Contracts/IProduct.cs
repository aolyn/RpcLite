using System;
using Model;

namespace Contracts
{
	public interface IProduct
	{
		int Add(Product product);
		int AddProduct(int id, Product product);
		int AddProduct(int id, Product product, int a, int b, int c, int d);
		void Delete(int id);
		Product[] Get();
		Product GetById(int id);
		Product Get(int id);

		//IAsyncResult BeginAdd(Product product, AsyncCallback cb, object state);
		//int EndAdd(IAsyncResult asyncResult);
	}
}
