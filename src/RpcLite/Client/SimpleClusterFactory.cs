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
		public IClientChannelFactory ChannelFactory { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public IRegistry Registry { get; set; }

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

	}
}
