using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RpcLite.Registry;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TContract"></typeparam>
	public class DefaultCluster<TContract> : ICluster<TContract>
	{
		private string _address;
		private readonly IRegistry _registry;
		private IClientChannel _channel;
		private readonly IClientChannelFactory _channelFactory;

		///// <summary>
		///// 
		///// </summary>
		//IClientChannelFactory ChannelFactory { get; set; }

		/// <summary>
		/// address or registry must provider one
		/// </summary>
		/// <param name="address"></param>
		/// <param name="registry"></param>
		/// <param name="channelFactory"></param>
		public DefaultCluster(string address, IRegistry registry, IClientChannelFactory channelFactory)
		{
			_channelFactory = channelFactory;
			//if (address == null && registry == null)
			//	throw new ArgumentOutOfRangeException(nameof(address), $"{nameof(address)} or {nameof(registry)} must provider one");

			_address = address;
			_registry = registry;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Address
		{
			get { return _address ?? _registry?.LookupAsync<TContract>().Result?.FirstOrDefault(); }
			set
			{
				_address = value;
				if (_channel != null)
					_channel.Address = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="content"></param>
		/// <param name="headers"></param>
		/// <returns></returns>
		public Task<ResponseMessage> SendAsync(string action, Stream content, IDictionary<string, string> headers)
		{
			if (_channel == null)
				_channel = _channelFactory.GetClientChannel(Address);
			return _channel.SendAsync(action, content, headers);
		}

	}
}
