using System.Threading.Tasks;
using Contracts;

namespace ServiceImpl
{
	public class PersonService : IPersonService
	{
		public int GetAge()
		{
			return 18;
		}

		public Task<int> GetAgeAsync()
		{
			return Task.FromResult(18);
		}

		public void SetAge(int age)
		{
		}

		public Task SetAgeAsync(int age)
		{
			return Task.FromResult<object>(null);
		}
	}
}
