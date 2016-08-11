using System;
using System.Threading.Tasks;
using org.apache.zookeeper;
using RpcLite.Config;

namespace RpcLite.Registry.Zookeeper
{
	public class ZookeeperRegistry : IRegistry
	{
		private ZookeeperRegistryInternal _zookeeper;
		private string _registryAddress;

		public bool CanRegister => true;

		public ZookeeperRegistry(string address)
			: this(address, 30 * 1000)
		{
		}

		public ZookeeperRegistry()
		{
			Initialize(RpcLiteConfig.Instance.Registry.Address, 30 * 1000);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <param name="expire">session timeout in milliseconds</param>
		public ZookeeperRegistry(string address, int expire)
		{
			Initialize(address, expire);
		}

		private void Initialize(string address, int expire)
		{
			_registryAddress = address;
			_zookeeper = new ZookeeperRegistryInternal(_registryAddress, expire);
		}

		public void Dispose()
		{
			_zookeeper.Dispose();
		}

		public async Task<Uri[]> LookupAsync(ClientConfigItem clientInfo)
		{
			try
			{
				var result = await _zookeeper.LookupAsync(clientInfo);
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
			//catch (Exception ex)
			//{
			//	throw;
			//}
		}

		public async Task RegisterAsync(ServiceConfigItem serviceInfo)
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
				else if (ex.getCode() == KeeperException.Code.CONNECTIONLOSS)
				{
					throw new ConnectionLossException(ex.Message, ex);
				}

				throw new ZookeeperException(ex.Message, ex);
			}

		}
	}
}
