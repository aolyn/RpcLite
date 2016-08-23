using System;
using System.Threading.Tasks;

namespace ServiceTest.Contract
{
	public interface IProductService
	{
		int GetCount();

		Task<int> GetCountAsync();

		void SetCount(int age);

		Task SetCountAsync(int age);

		Product[] GetAll();

		Product[] GetPage(int pageIndex, int pageSize);

		Task<Product[]> GetAllAsync();

		int Add(Product product);

		Task<int> AddAsync(Product product);

		Product GetById(int id);

		Task<Product> GetByIdAsync(int id);

		void ExceptionTest();

		void ThrowException(Exception ex);

		Task ExceptionTestAsync();
	}
}
