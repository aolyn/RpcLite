using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using org.apache.zookeeper;
using RpcLite.Config;
using RpcLite.Utility;

namespace RpcLite.Registry.Zookeeper
{
	public class ZookeeperRegistry : Watcher, IRegistry
	{
		private static QuickReadConcurrentDictionary<Type, string> _defaultBaseUrlDictionary = new QuickReadConcurrentDictionary<Type, string>();
		IZookeeper _zookeeper;
		Task _startTask;

		public ZookeeperRegistry(string address)
		{
			_startTask = Start(address);
		}

		public ZookeeperRegistry()
		{
			if (RpcLiteConfig.Instance?.Registry != null)
			{
				var address = RpcLiteConfig.Instance.Registry.Address;
				_startTask = Start(address);
			}
		}

		public bool CanRegister => true;

		public async Task<Uri[]> LookupAsync(ClientConfigItem clientInfo)
		{
			EnsureStartComplete();

			var appId = clientInfo.Name;

			var appNodePath = GetAppNodePath(appId);
			//var appNode = await _zookeeper.ExistsAsync(appNodePath, false).ConfigureAwait(false);
			//if (appNode == null)
			//{
			//	return new Uri[0];
			//}

			var envNodePath = appNodePath + "/" + (string.IsNullOrWhiteSpace(clientInfo.Environment) ? "_" : clientInfo.Environment);
			//var envNode = await _zookeeper.ExistsAsync(envNodePath, false).ConfigureAwait(false);
			//if (envNode == null)
			//{
			//	return new Uri[0];
			//}

			var serviceNodes = await _zookeeper.GetChildrenAsync(envNodePath, true).ConfigureAwait(false);
			if (serviceNodes?.Children?.Count > 0)
			{
				var addresses = new List<Uri>();
				foreach (var item in serviceNodes.Children)
				{
					var addr = await _zookeeper.GetDataAsync(envNodePath + "/" + item, true).ConfigureAwait(false);
					var addrString = Encoding.UTF8.GetString(addr.Data);
					addresses.Add(new Uri(addrString));
				}
				return addresses.ToArray();
			}

			return new Uri[0];
		}

		private void EnsureStartComplete()
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


		public async Task RegisterAsync(ServiceConfigItem serviceInfo)
		{
			if (string.IsNullOrWhiteSpace(serviceInfo?.Address)) return;

			EnsureStartComplete();
			var appId = serviceInfo.Name;

			string appNodePath = GetAppNodePath(appId);
			var appNode = await _zookeeper.ExistsAsync(appNodePath, false).ConfigureAwait(false);
			if (appNode == null)
			{
				var appNodeName = await _zookeeper.CreateAsync(appNodePath, null, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT).ConfigureAwait(false);
			}

			var envNodePath = appNodePath + "/" + (string.IsNullOrWhiteSpace(serviceInfo.Environment) ? "_" : serviceInfo.Environment);
			var envNode = await _zookeeper.ExistsAsync(envNodePath, false).ConfigureAwait(false);
			if (envNode == null)
			{
				var envNodeName = await _zookeeper.CreateAsync(envNodePath, null, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT).ConfigureAwait(false);
			}

			var serviceNodePath = envNodePath + "/service-";
			var serviceNode = await _zookeeper.CreateAsync(serviceNodePath,
				serviceInfo.Address.GetBytes(), ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL_SEQUENTIAL).ConfigureAwait(false);

			//var serviceNode = await _zookeeper.CreateAsync(serviceNodePath,
			//	JsonHelper.Serialize(address).GetBytes(), ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL_SEQUENTIAL);
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
			return Task.FromResult<object>(null);
		}

		private static readonly string ServiceRootPath = @"/rpclite-services";

		private async Task Start(string address)
		{
			//_zookeeper = new Zookeeper("192.168.9.1:2181", 3600 * 1000, this);
			_zookeeper = new Zookeeper(address, 3600 * 1000, this);

			var root = await _zookeeper.ExistsAsync(ServiceRootPath, false).ConfigureAwait(false);

			if (root == null)
			{
				////创建一个节点root，数据是mydata,不进行ACL权限控制，节点为永久性的(即客户端shutdown了也不会消失) 
				var rv1 = await _zookeeper.CreateAsync(ServiceRootPath, "rpclite service root node".GetBytes(),
					ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT).ConfigureAwait(false);
			}

			//Console.ReadLine();
		}
	}

}
