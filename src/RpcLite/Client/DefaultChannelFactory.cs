using System;
using System.Collections.Generic;
using RpcLite.Config;
using RpcLite.Logging;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	internal class DefaultChannelFactory : IChannelFactory
	{
		private readonly Dictionary<string, IChannelProvider> _chanelProviderDictionary = new Dictionary<string, IChannelProvider>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public IChannel GetChannel(string address)
		{
			try
			{
				var uri = new Uri(address);
				IChannelProvider provider;
				if (_chanelProviderDictionary.TryGetValue(uri.Scheme.ToLower(), out provider))
					return provider.GetChannel(address);
			}
			catch (Exception ex)
			{
				LogHelper.Error(ex);
			}
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		public void Initialize(RpcConfig config)
		{
			var channelProviders = new List<IChannelProvider> { new DefaultChannelProvider() };

			if (config?.Client?.Channel?.Providers != null)
			{
				foreach (var provider in config.Client.Channel.Providers)
				{
					channelProviders.Add(TypeCreator.CreateInstanceByIdentifier<IChannelProvider>(provider.Type));
				}
			}

			foreach (var provider in channelProviders)
			{
				foreach (var protocolName in provider.Protocols)
				{
					_chanelProviderDictionary[protocolName.ToLower()] = provider;
				}
			}
		}

	}
}
