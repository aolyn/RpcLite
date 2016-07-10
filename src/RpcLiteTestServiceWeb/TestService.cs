using System;
using System.Globalization;
using System.Threading.Tasks;
using RpcLiteTestService.Contract;

namespace RpcLiteTestServiceWeb
{
	public class TestService : ITestService
	{
		public string GetDateTimeString()
		{
			return DateTime.Now.ToString(CultureInfo.InvariantCulture);
		}

		public void SetAge(int a)
		{
			Console.WriteLine("void SetAge(int a)");
		}

		public Task SetAgeAsync(int a)
		{
			Console.WriteLine("Task SetAgeAsync(int a)");
			return Task.FromResult<object>(null);
		}

		public Task<int> AddAsync(int a, int b)
		{
			Console.WriteLine("Task<int> AddAsync(int a, int b)");
			return Task.FromResult(a + b);
		}
	}
}
