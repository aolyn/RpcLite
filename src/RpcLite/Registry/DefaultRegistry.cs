using System;
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
		private IDictionary<ServiceIdentifier, ServiceInfo[]> _defaultBaseUrlDictionary;

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
			var tempDic = GetAddresses<Dictionary<ServiceIdentifier, ServiceInfo[]>>(Config);
			_defaultBaseUrlDictionary = tempDic;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		public override Task<ServiceInfo[]> LookupAsync(string name, string group)
		{
			ServiceInfo[] result;
			_defaultBaseUrlDictionary.TryGetValue(new ServiceIdentifier(name, group), out result);
			return TaskHelper.FromResult(result);
		}

		/// <summary>
		/// 
		/// </summary>
		public override bool CanRegister => false;

		//private string[] GetAddressInternal(ClientConfigItem clientInfo)
		//{
		//	string url;
		//	return _defaultBaseUrlDictionary.TryGetValue(clientInfo, out url)
		//		? new[] { url }
		//		: null;
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceInfo"></param>
		/// <returns></returns>
		public Task RegisterAsync(ServiceConfigItem serviceInfo)
		{
			return TaskHelper.FromResult<object>(null);
		}

		public override Task RegisterAsync(ServiceInfo serviceInfo)
		{
			throw new NotImplementedException();
		}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="clientInfo"></param>
		///// <returns></returns>
		//public Task<string[]> LookupAsync(ClientConfigItem clientInfo)
		//{
		//	return TaskHelper.FromResult(GetAddressInternal(clientInfo));
		//}

	}

}
