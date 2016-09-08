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
		public abstract Task RegisterAsync(ServiceConfigItem serviceInfo);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="clientInfo"></param>
		/// <returns></returns>
		public abstract Task<string[]> LookupAsync(ClientConfigItem clientInfo);

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContract"></typeparam>
		/// <returns></returns>
		public Task<string[]> LookupAsync<TContract>()
		{
			var type = typeof(TContract);
			var clientConfigItem = Config?.Client?.Clients
				.FirstOrDefault(it => it.TypeName == type.FullName);

			return clientConfigItem == null
				? TaskHelper.FromResult<string[]>(null)
				: LookupAsync(clientConfigItem);
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
			where TDictionary : IDictionary<ClientConfigItem, string>, new()
		{
			return RegistryHelper.GetAddresses<TDictionary>(config);
		}

	}
}
