using Model;

namespace Contracts
{
	public interface IPerson
	{
		int GetAge();
		void Breath();
		int AddProduct(int id, Product product);
		void Delete(int id);
		Product[] Get();
		Product GetById(int id);
		Product Get(int id);
	}
}
