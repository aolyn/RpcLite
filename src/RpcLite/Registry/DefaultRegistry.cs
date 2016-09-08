using System.Collections.Generic;
using System.Threading.Tasks;
using RpcLite.Config;

namespace RpcLite.Registry
{
	/// <summary>
	/// only get address from config
	/// </summary>
	public class DefaultRegistry : RegistryBase
	{
		private IDictionary<ClientConfigItem, string> _defaultBaseUrlDictionary;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		public DefaultRegistry(RpcConfig config)
			: base(config)
		{
			InitilizeAddresses();
		}

		private void InitilizeAddresses()
		{
			var tempDic = GetAddresses<Dictionary<ClientConfigItem, string>>(Config);
			_defaultBaseUrlDictionary = tempDic;
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool CanRegister => false;

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
		public override Task RegisterAsync(ServiceConfigItem serviceInfo)
		{
			return TaskHelper.FromResult<object>(null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clientInfo"></param>
		/// <returns></returns>
		public override Task<string[]> LookupAsync(ClientConfigItem clientInfo)
		{
			return TaskHelper.FromResult(GetAddressInternal(clientInfo));
		}

	}

}
