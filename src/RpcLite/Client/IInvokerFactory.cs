using RpcLite.Registry;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IInvokerFactory
	{

		///// <summary>
		///// 
		///// </summary>
		//IClientChannelFactory ChannelFactory { get; set; }

		///// <summary>
		///// 
		///// </summary>
		//IRegistry Registry { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registry"></param>
		/// <param name="channelFactory"></param>
		void Initilize(IRegistry registry, IChannelFactory channelFactory);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		IInvoker GetInvoker(string address);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IInvoker GetInvoker();
	}
}
