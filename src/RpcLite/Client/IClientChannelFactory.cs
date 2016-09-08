namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IClientChannelFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		IClientChannel GetClientChannel(string address);
	}
}
