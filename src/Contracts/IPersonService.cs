using System.Threading.Tasks;

namespace Contracts
{
	public interface IPersonService
	{
		int GetAge();
		void SetAge(int age);

		Task<int> GetAgeAsync();
		Task SetAgeAsync(int age);
	}
}
