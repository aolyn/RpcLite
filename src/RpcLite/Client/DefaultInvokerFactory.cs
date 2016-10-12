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
		public IChannelFactory ChannelFactory { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public IRegistry Registry { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public IInvoker<TContract> GetInvoker<TContract>(string address)
		{
			return new DefaultInvoker<TContract>(address, Registry, ChannelFactory);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IInvoker<TContract> GetInvoker<TContract>()
		{
			return GetInvoker<TContract>(null);
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
