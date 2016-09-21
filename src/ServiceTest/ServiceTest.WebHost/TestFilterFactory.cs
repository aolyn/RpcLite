using RpcLite;

namespace ServiceTest.WebHost
{
	public class TestFilterFactory : IFilterFactory
	{
		public IRpcFilter[] CreateFilters()
		{
			return new IRpcFilter[]
			{
				new LogRequestTimeFilter(),
				//new LogTimeFilter(),
				//new ResultFilter(),
			};
		}
	}
}