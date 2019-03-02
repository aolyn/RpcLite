using System;
using System.Threading.Tasks;

namespace ServiceTest.UnitTests.Basics
{
	public interface IProductService
	{
		int GetNumber();

		Task<int> GetNumberAsync();


		void SetNumber(int num);

		Task SetNumberAsync(int num);


		Product[] GetAll();

		Task<Product[]> GetAllAsync();


		Product GetInput(Product product);

		Task<Product> GetInputAsync(Product product);


		Product[] GetPage(int pageIndex, int pageSize);

		Task<Product[]> GetPageAsync(int pageIndex, int pageSize);


		int Add(Product product);

		Task<int> AddAsync(Product product);


		Product GetById(int id);

		Task<Product> GetByIdAsync(int id);


		void ExceptionTest();

		void ThrowException(Exception ex);

		Task ExceptionTestAsync();
	}
}
