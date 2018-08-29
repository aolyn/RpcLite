using RpcLite.Config;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public interface IServiceMapperFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		IServiceMapper CreateServiceMapper(RpcServiceFactory factory, ServiceConfig config);
	}
}
