namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IChannelProvider
	{
		/// <summary>
		/// friendly name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// support protocols
		/// </summary>
		string[] Protocols { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		IChannel GetChannel(string address);
	}
}
