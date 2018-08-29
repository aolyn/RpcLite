using RpcLite.Config;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class DefaultServiceMapperFactory : IServiceMapperFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public IServiceMapper CreateServiceMapper(RpcServiceFactory factory, ServiceConfig config)
		{
			return new DefaultServiceMapper(factory);
		}
	}
}
