namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public class DefaultChannelProvider : IChannelProvider
	{
		/// <summary>
		/// 
		/// </summary>
		public virtual string Name { get; } = nameof(DefaultChannelProvider);

		/// <summary>
		/// 
		/// </summary>
		public virtual string[] Protocols { get; } = { "http", "https" };

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public virtual IChannel GetChannel(string address)
		{
			return new HttpClientChannel(address);
		}
	}
}
