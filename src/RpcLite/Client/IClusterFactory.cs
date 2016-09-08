using RpcLite.Registry;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IClusterFactory
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
		void Initilize(IRegistry registry, IClientChannelFactory channelFactory);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		ICluster<TContract> GetCluster<TContract>(string address);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		ICluster<TContract> GetCluster<TContract>();
	}
}
