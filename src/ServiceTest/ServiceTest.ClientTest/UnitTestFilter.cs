using RpcLite;
using RpcLite.Client;

namespace ServiceTest.ClientTest
{
	public class UnitTestFilterFactory : IFilterFactory
	{
		public IRpcFilter[] CreateFilters()
		{
			return new IRpcFilter[]
			{
				new TestClientInvokeFilter()
			};
		}
	}

	class TestClientInvokeFilter : IClientInvokeFilter
	{
		public string Name { get; set; }
		public void OnInvoking(ClientContext context)
		{

		}

		public void OnInvoked(ClientContext context)
		{

		}
	}

}
