using RpcLite.Registry;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public class SimpleClusterFactory : IClusterFactory
	{
		/// <summary>
		/// 
		/// </summary>
		public IClientChannelFactory ChannelFactory { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public IRegistry Registry { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public ICluster<TContract> GetCluster<TContract>(string address)
		{
			return new DefaultCluster<TContract>(address, Registry, ChannelFactory);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ICluster<TContract> GetCluster<TContract>()
		{
			return GetCluster<TContract>(null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registry"></param>
		/// <param name="channelFactory"></param>
		public void Initilize(IRegistry registry, IClientChannelFactory channelFactory)
		{
			Registry = registry;
			ChannelFactory = channelFactory;
		}

	}
}
