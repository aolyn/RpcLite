using RpcLite.Config;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IChannelFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		IChannel GetChannel(string address);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		void Initialize(ClientConfig config);
	}
}
