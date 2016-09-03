using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpcLite.Config;

namespace RpcLite.Registry
{
	/// <summary>
	/// only get address from config
	/// </summary>
	public class DefaultRegistry : IRegistry
	{
		private IDictionary<ClientConfigItem, string> _defaultBaseUrlDictionary;
		private readonly RpcConfig _config;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		public DefaultRegistry(RpcConfig config)
		{
			_config = config;

			InitilizeAddresses();
		}

		private void InitilizeAddresses()
		{
			var tempDic = RegistryHelper.GetAddresses<Dictionary<ClientConfigItem, string>>(_config);
			_defaultBaseUrlDictionary = tempDic;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool CanRegister => false;

		private string[] GetAddressInternal(ClientConfigItem clientInfo)
		{
			string url;
			return _defaultBaseUrlDictionary.TryGetValue(clientInfo, out url)
				? new[] { url }
				: null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceInfo"></param>
		/// <returns></returns>
		public Task RegisterAsync(ServiceConfigItem serviceInfo)
		{
			return TaskHelper.FromResult<object>(null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clientInfo"></param>
		/// <returns></returns>
		public Task<string[]> LookupAsync(ClientConfigItem clientInfo)
		{
			return TaskHelper.FromResult(GetAddressInternal(clientInfo));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public Task<string[]> LookupAsync<TContract>()
		{
			var type = typeof(TContract);
			var clientConfigItem = _config.Client.Clients
				.FirstOrDefault(it => it.TypeName == type.FullName);

			return clientConfigItem == null
				? TaskHelper.FromResult<string[]>(null)
				: LookupAsync(clientConfigItem);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
		}

	}
}
