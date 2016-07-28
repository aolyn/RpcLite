using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.apache.zookeeper;
using RpcLite.Config;

namespace RpcLite.Registry.Zookeeper
{
	public class ZookeeperRegistry : Watcher, IRegistry
	{
		private static ConcurrentDictionary<string, Uri[]> _defaultBaseUrlDictionary = new ConcurrentDictionary<string, Uri[]>();
		private IZookeeper _zookeeper;
		private Task _startTask;
		private bool _isDisposed;
		private TimeSignalDelay _delay;
		private string _registryAddress;
		private static readonly string ServiceRootPath = @"/rpclite-services";
		private bool _isStarted;

		public ZookeeperRegistry(string address)
		{
			_registryAddress = address;
			_startTask = Start();
		}

		public ZookeeperRegistry()
		{
			if (RpcLiteConfig.Instance?.Registry != null)
			{
				_registryAddress = RpcLiteConfig.Instance.Registry.Address;
				_startTask = Start();
			}
		}

		~ZookeeperRegistry()
		{
			Dispose();
		}

		public bool CanRegister => true;

		public async Task<Uri[]> LookupAsync(ClientConfigItem clientInfo)
		{
			if (clientInfo == null) return null;

			EnsureStartComplete();

			var key = clientInfo.Name + "/" + clientInfo.Environment;
			var url = _defaultBaseUrlDictionary.GetOrAdd(key, _key =>
			{
				var urls = LookupInternalAsync(clientInfo);
				return urls.Result;
			});

			return url;
		}

		public async Task<Uri[]> LookupInternalAsync(ClientConfigItem clientInfo)
		{
			var appId = clientInfo.Name;

			var appNodePath = GetAppNodePath(appId);
			var envNodePath = appNodePath + "/" + (string.IsNullOrWhiteSpace(clientInfo.Environment) ? "_" : clientInfo.Environment);

			var serviceNodes = await _zookeeper.GetChildrenAsync(envNodePath, true).ConfigureAwait(false);
			if (serviceNodes?.Children?.Count > 0)
			{
				var addresses = new List<string>();
				foreach (var item in serviceNodes.Children)
				{
					var addr = await _zookeeper.GetDataAsync(envNodePath + "/" + item, true).ConfigureAwait(false);
					var addrString = Encoding.UTF8.GetString(addr.Data);
					addresses.Add(addrString);
				}
				return addresses
					.Distinct()
					.Select(it => new Uri(it))
					.ToArray();
			}

			return new Uri[0];
		}

		private void EnsureStartComplete()
		{
			if (!_startTask.IsCompleted)
				_startTask.Wait();
		}

		public async Task RegisterAsync(ServiceConfigItem serviceInfo)
		{
			if (string.IsNullOrWhiteSpace(serviceInfo?.Address))
				return;

			try
			{
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
			}
			catch (KeeperException ex)
			{
				if (ex.getCode() == KeeperException.Code.CONNECTIONLOSS)
				{
				}
				else if (ex.getCode() == KeeperException.Code.CONNECTIONLOSS)
				{
				}
				throw;
			}
			catch (Exception)
			{
				throw;
			}

		}

		private static string GetAppNodePath(string appId)
		{
			return ServiceRootPath + "/" + appId;
		}

		public void Dispose()
		{
			_isDisposed = true;
			_zookeeper?.CloseAsync().Wait();
		}

		public override Task process(WatchedEvent @event)
		{
			switch (@event.get_Type())
			{
				case Event.EventType.NodeChildrenChanged:
					break;
			}
			return Task.FromResult<object>(null);
		}

		private async Task Start()
		{
			if (_isStarted)
				return;

			if (_zookeeper != null)
				await _zookeeper.CloseAsync();

			_zookeeper = new Zookeeper(_registryAddress, 3600 * 1000, this);
			//_zookeeper = new Zookeeper("192.168.9.1:2181", 3600 * 1000, this);
			try
			{
				var root = await _zookeeper.ExistsAsync(ServiceRootPath, false).ConfigureAwait(false);

				if (root == null)
				{
					try
					{
						////创建一个节点root，数据是mydata,不进行ACL权限控制，节点为永久性的(即客户端shutdown了也不会消失) 
						var rv1 = await _zookeeper.CreateAsync(ServiceRootPath, "rpclite service root node".GetBytes(),
							ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT).ConfigureAwait(false);
					}
					catch (KeeperException ex)
					{
						if (ex.getCode() != KeeperException.Code.NODEEXISTS)
							throw;
					}
				}

				_isStarted = true;
			}
			catch (KeeperException)
			{
				throw;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}

}
