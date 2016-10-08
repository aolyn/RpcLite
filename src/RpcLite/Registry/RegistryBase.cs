using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpcLite.Config;

namespace RpcLite.Registry
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class RegistryBase : IRegistry
	{
		/// <summary>
		/// 
		/// </summary>
		protected RpcConfig Config;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		protected RegistryBase(RpcConfig config)
		{
			Config = config;
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual void Dispose() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceInfo"></param>
		/// <returns></returns>
		public abstract Task RegisterAsync(ServiceInfo serviceInfo);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public abstract Task<ServiceInfo[]> LookupAsync(string name, string group);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clientConfigItem"></param>
		/// <returns></returns>
		protected Task<ServiceInfo[]> LookupAsync(ClientConfigItem clientConfigItem)
		{
			return clientConfigItem == null
				? TaskHelper.FromResult<ServiceInfo[]>(null)
				: LookupAsync(clientConfigItem.Name, clientConfigItem.Group);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public Task<ServiceInfo[]> LookupAsync<TContract>()
		{
			var type = typeof(TContract);
			var clientConfigItem = Config?.Client?.Clients
				.FirstOrDefault(it => it.TypeName == type.FullName);

			return LookupAsync(clientConfigItem);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">service name</param>
		/// <returns></returns>
		public Task<ServiceInfo[]> LookupAsync(string name)
		{
			var clientConfigItem = Config?.Client?.Clients
				.FirstOrDefault(it => it.Name == name);

			return LookupAsync(clientConfigItem);
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract bool CanRegister { get; }

		/// <summary>
		/// get address from config to Dictionary
		/// </summary>
		/// <typeparam name="TDictionary"></typeparam>
		/// <param name="config"></param>
		/// <returns></returns>
		public static TDictionary GetAddresses<TDictionary>(RpcConfig config)
			where TDictionary : IDictionary<ServiceIdentifier, ServiceInfo[]>, new()
		{
			return RegistryHelper.GetAddresses<TDictionary>(config);
		}

	}
}
