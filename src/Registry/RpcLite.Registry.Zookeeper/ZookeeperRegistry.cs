using System.Linq;
using System.Threading.Tasks;
using org.apache.zookeeper;
using RpcLite.Config;

namespace RpcLite.Registry.Zookeeper
{
	public class ZookeeperRegistry : IRegistry
	{
		private ZookeeperRegistryInternal _zookeeper;
		private string _registryAddress;
		private readonly RpcConfig _config;

		public bool CanRegister => true;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <param name="expire">session timeout in milliseconds</param>
		public ZookeeperRegistry(string address, int expire)
		{
			Initialize(address, expire);
		}

		public ZookeeperRegistry(RpcConfig config)
		{
			_config = config;
			_registryAddress = config.Registry.Address;
			var sessionExpire = 30 * 1000;
			Initialize(_registryAddress, sessionExpire);
		}

		private void Initialize(string address, int expire)
		{
			_registryAddress = address;
			_zookeeper = new ZookeeperRegistryInternal(_registryAddress, expire);
		}

		public async Task<ServiceInfo[]> LookupAsync(string name, string group)
		{
			try
			{
				var result = await _zookeeper.LookupAsync(name, group);
				return result;
			}
			catch (KeeperException ex)
			{
				if (ex.getCode() == KeeperException.Code.SESSIONEXPIRED)
				{
					throw new SessionExpireException(ex.Message, ex);
				}

				throw new ZookeeperException(ex.Message, ex);
			}
		}

		public void Dispose()
		{
			_zookeeper.Dispose();
		}

		public Task<ServiceInfo[]> LookupAsync(ClientConfigItem clientInfo)
		{
			return clientInfo != null
				? LookupAsync(clientInfo.Name, clientInfo.Group)
				: null;
		}

		public async Task RegisterAsync(ServiceInfo serviceInfo)
		{
			try
			{
				await _zookeeper.RegisterAsync(serviceInfo);
			}
			catch (KeeperException ex)
			{
				if (ex.getCode() == KeeperException.Code.SESSIONEXPIRED)
				{
					throw new SessionExpireException(ex.Message, ex);
				}
				if (ex.getCode() == KeeperException.Code.CONNECTIONLOSS)
				{
					throw new ConnectionLossException(ex.Message, ex);
				}

				throw new ZookeeperException(ex.Message, ex);
			}
		}

		public Task<ServiceInfo[]> LookupAsync(string name)
		{
			return LookupAsync(name, null);
		}

		public Task<ServiceInfo[]> LookupAsync<TContract>()
		{
			var type = typeof(TContract);
			var clientConfigItem = _config.Client.Clients
				.FirstOrDefault(it => it.TypeName == type.FullName);

			return LookupAsync(clientConfigItem);
		}

	}
}
