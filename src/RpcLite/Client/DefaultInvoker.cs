﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RpcLite.Registry;
using RpcLite.Service;

namespace RpcLite.Client
{
	/// <inheritdoc />
	public class DefaultInvoker : InvokerBase
	{
		private string _address;
		private readonly IRegistry _registry;
		private IChannel _channel;
		private readonly IChannelFactory _channelFactory;

		/// <summary>
		/// address or registry must provider one
		/// </summary>
		/// <param name="address"></param>
		/// <param name="registry"></param>
		/// <param name="channelFactory"></param>
		public DefaultInvoker(string address, IRegistry registry, IChannelFactory channelFactory)
		{
			_channelFactory = channelFactory;
			_address = address;
			_registry = registry;
		}

		/// <inheritdoc />
		public override string Address
		{
			get
			{
				return _address ?? GetAddressFromRegistry();
			}
			set
			{
				_address = value;
				if (_channel == null) return;

				Uri oldUri, newUri;

				var oldCreateResult = Uri.TryCreate(_channel.Address, UriKind.Absolute, out oldUri);
				var newCreateResult = Uri.TryCreate(value, UriKind.Absolute, out newUri);

				if ((oldCreateResult && newCreateResult && !newUri.Scheme.Equals(oldUri.Scheme, StringComparison.OrdinalIgnoreCase))
					|| oldCreateResult || newCreateResult)
				{
					_channel = null;
					return;
				}

				if (_channel != null)
					_channel.Address = value;
			}
		}

		private string GetAddressFromRegistry()
		{
			return _registry?.LookupAsync(Name, Group)?.Result
				?.Select(it => it.Address)
				.FirstOrDefault();
		}

		/// <inheritdoc />
		protected override Task<IResponseMessage> SendAsync(string action, Stream content,
			IDictionary<string, string> headers)
		{
			if (string.IsNullOrEmpty(Address))
				throw new ServiceException("service address not provided");

			if (_channel == null)
				_channel = _channelFactory.GetChannel(Address);
			return _channel.SendAsync(action, content, headers);
		}

	}
}
