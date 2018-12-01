namespace RpcLite.Client
{
	/// <inheritdoc />
	public class DefaultChannelProvider : IChannelProvider
	{
		/// <inheritdoc />
		public virtual string Name { get; } = nameof(DefaultChannelProvider);

		/// <inheritdoc />
		public virtual string[] Protocols { get; } = { "http", "https" };

		/// <inheritdoc />
		public virtual IChannel GetChannel(string address)
		{
			return new HttpClientChannel(address);
		}
	}
}
