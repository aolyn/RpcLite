using System.Threading.Tasks;

namespace RpcLiteTestService.Contract
{
	public interface ITestService
	{
		string GetDateTimeString();

		void SetAge(int a);

		Task SetAgeAsync(int a);

		Task<int> AddAsync(int a, int b);
	}
}
