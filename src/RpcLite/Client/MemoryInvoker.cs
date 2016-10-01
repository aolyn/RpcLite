using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TContract"></typeparam>
	public class MemoryInvoker<TContract> : InvokerBase<TContract>
	{
		private string _address;
		private readonly IClientChannel _channel;

		/// <summary>
		/// address or registry must provider one
		/// </summary>
		/// <param name="appHost"></param>
		/// <param name="address"></param>
		public MemoryInvoker(AppHost appHost, string address)
		{
			_address = address;
			_channel = new MemoryClientChannel(appHost) { Address = _address };
		}

		/// <summary>
		/// 
		/// </summary>
		public override string Address
		{
			get { return _address; }
			set { _address = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="content"></param>
		/// <param name="headers"></param>
		/// <returns></returns>
		protected override Task<IResponseMessage> SendAsync(string action, Stream content, IDictionary<string, string> headers)
		{
			_channel.Address = Address;
			return _channel.SendAsync(action, content, headers);
		}

	}
}
