using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using org.apache.zookeeper;
using RpcLite.Utility;

namespace RpcLite.Registry.Zookeeper
{
	public class ZookeeperRegistry : Watcher, IRegistry
	{
		IZookeeper _zookeeper;
		private static QuickReadConcurrentDictionary<Type, string> _defaultBaseUrlDictionary = new QuickReadConcurrentDictionary<Type, string>();
		Task _startTask;
		public ZookeeperRegistry(string address)
		{
			_startTask = Start(address);
		}

		public bool CanRegister => true;

		public async Task<string[]> LookupAsync(string appId)
		{
			WaitStartComplete();

			string appNodePath = GetAppNodePath(appId);
			var appNode = await _zookeeper.ExistsAsync(appNodePath, false);
			if (appNode == null)
			{
				return null;
			}

			var serviceNodes = await _zookeeper.GetChildrenAsync(appNodePath, true);
			if (serviceNodes?.Children?.Count > 0)
			{
				var addresses = new List<string>();
				foreach (var item in serviceNodes.Children)
				{
					var addr = await _zookeeper.GetDataAsync(appNodePath + "/" + item, true);
					var addrString = Encoding.UTF8.GetString(addr.Data);
					addresses.Add(addrString);
				}
				return addresses.ToArray();
			}

			return new string[0];
		}

		private void WaitStartComplete()
		{
			if (!_startTask.IsCompleted)
				_startTask.Wait();
		}

		//private static Uri GetAddressInternal(Type type)
		//{
		//	// ReSharper disable once InconsistentlySynchronizedField
		//	var url = _defaultBaseUrlDictionary.GetOrAdd(type, () =>
		//	{
		//		var clientConfigItem = RpcLiteConfig.Instance.Clients
		//			.FirstOrDefault(it => it.TypeName == type.FullName);

		//		if (clientConfigItem == null) return null;
		//		if (RegistryClient.Value == null) return null;

		//		var request = new GetServiceAddressRequest
		//		{
		//			ServiceName = clientConfigItem.Name,
		//			Namespace = clientConfigItem.Namespace,
		//			Environment = RpcLiteConfig.Instance.ClientEnvironment,
		//		};
		//		var response = RegistryClient.Value.GetServiceAddress(request);
		//		var uri = string.IsNullOrWhiteSpace(response?.Address)
		//			? null
		//			: response.Address;

		//		return uri;
		//	});

		//	return string.IsNullOrEmpty(url)
		//		? null
		//		: new Uri(url);
		//}

		public async Task RegisterAsync(string appId, string[] address)
		{
			WaitStartComplete();

			string appNodePath = GetAppNodePath(appId);
			var appNode = await _zookeeper.ExistsAsync(appNodePath, false);
			if (appNode == null)
			{
				var appNodeName = await _zookeeper.CreateAsync(appNodePath, null, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
			}

			var serviceNodePath = appNodePath + "/service-";
			var serviceNode = await _zookeeper.CreateAsync(serviceNodePath,
				JsonHelper.Serialize(address).GetBytes(), ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL_SEQUENTIAL);
		}

		private static string GetAppNodePath(string appId)
		{
			return ServiceRootPath + "/" + appId;
		}

		public void Dispose()
		{
			_zookeeper?.CloseAsync().Wait();
		}

		public override Task process(WatchedEvent @event)
		{
			throw new NotImplementedException();
		}

		private static readonly string ServiceRootPath = @"/rpclite-services";

		private async Task Start(string address)
		{
			//_zookeeper = new Zookeeper("192.168.9.1:2181", 3600 * 1000, this);
			_zookeeper = new Zookeeper(address, 3600 * 1000, this);

			var root = await _zookeeper.ExistsAsync(ServiceRootPath, false);

			if (root == null)
			{
				////创建一个节点root，数据是mydata,不进行ACL权限控制，节点为永久性的(即客户端shutdown了也不会消失) 
				var rv1 = await _zookeeper.CreateAsync(ServiceRootPath, "rpclite service root node".GetBytes(), ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
			}


			Console.ReadLine();
		}

	}

}
