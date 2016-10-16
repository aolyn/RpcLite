using RpcLite.Registry;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public class DefaultInvokerFactory : IInvokerFactory
	{
		/// <summary>
		/// 
		/// </summary>
		private IChannelFactory ChannelFactory { get; set; }

		/// <summary>
		/// 
		/// </summary>
		private IRegistry Registry { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public IInvoker GetInvoker(string address)
		{
			return new DefaultInvoker(address, Registry, ChannelFactory);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IInvoker GetInvoker()
		{
			return GetInvoker(null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registry"></param>
		/// <param name="channelFactory"></param>
		public void Initilize(IRegistry registry, IChannelFactory channelFactory)
		{
			Registry = registry;
			ChannelFactory = channelFactory;
		}

	}
}
