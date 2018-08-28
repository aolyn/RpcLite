using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.apache.zookeeper;
using RpcLite.Logging;

namespace RpcLite.Registry.Zookeeper
{
	internal class ZookeeperRegistryInternal : Watcher
	{
		private readonly ConcurrentDictionary<ServiceIdentifier, ClientLookupItem> _serviceAddressDictionary =
			new ConcurrentDictionary<ServiceIdentifier, ClientLookupItem>();
		private readonly ConcurrentDictionary<ServiceInfo, DateTime> _registerServiceDictionary =
			new ConcurrentDictionary<ServiceInfo, DateTime>();

		private IZookeeper _zookeeper;
		private Task _startTask;
		private bool _isDisposed;
		private readonly string _registryAddress;
		private static readonly string ServiceRootPath = @"/rpclite-services";
		private readonly int _sessionTimeout;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <param name="expire">session expire in milliseconds</param>
		public ZookeeperRegistryInternal(string address, int expire)
		{
			if (string.IsNullOrWhiteSpace(address))
				throw new ArgumentNullException(nameof(address));

			_registryAddress = address;
			_sessionTimeout = expire;
			_startTask = StartAsync();
		}

		//public ZookeeperRegistryInternal()
		//	: this(RpcLiteConfig.Instance?.Registry?.Address, 30 * 1000)
		//{ }

		~ZookeeperRegistryInternal()
		{
			Dispose();
		}

		public bool CanRegister => true;

		public Task<ServiceInfo[]> LookupAsync(string name, string group)
		{
			if (name == null) return Task.FromResult((ServiceInfo[])null);

			EnsureStartComplete();

			var key = new ServiceIdentifier(name, group);
			var url = _serviceAddressDictionary.GetOrAdd(key, key1 =>
			{
				try
				{
					var urls = LookupInternalAsync(key1);
					var item = new ClientLookupItem
					{
						ClientInfo = key1,
						Addresses = urls.Result,
					};
					return item;
				}
				catch (AggregateException ex)
				{
					var kex = ex.InnerException as KeeperException;
					if (kex?.getCode() == KeeperException.Code.SESSIONEXPIRED)
					{
						if (_startTask?.IsCompleted == true)
							_startTask = StartAsync();
					}

					throw ex.InnerException;
				}
			});

			return Task.FromResult(url?.Addresses);
		}

		public async Task<ServiceInfo[]> LookupInternalAsync(ServiceIdentifier clientInfo)
		{
			try
			{
				var appId = clientInfo.Name;

				var appNodePath = GetAppNodePath(appId);
				var envNodePath = appNodePath + "/" + (string.IsNullOrWhiteSpace(clientInfo.Group) ? "_" : clientInfo.Group);

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
						.Select(it => new ServiceInfo { Name = clientInfo.Name, Group = clientInfo.Group, Address = it })
						.ToArray();
				}

				return new ServiceInfo[0];
			}
			catch (KeeperException)
			{
				//if (ex.getCode() == KeeperException.Code.SESSIONEXPIRED)
				//{
				//	throw new SessionExpireException(ex.Message, ex);
				//}
				//else if (ex.getCode() == KeeperException.Code.CONNECTIONLOSS)
				//{
				//	throw new ConnectionLossException(ex.Message, ex);
				//}
				throw;
			}
			catch (Exception ex)
			{
				LogHelper.Error(ex);
				throw;
			}

		}

		private void EnsureStartComplete()
		{
			_startTask?.Wait();
		}

		public async Task RegisterAsync(ServiceInfo serviceInfo)
		{
			if (string.IsNullOrWhiteSpace(serviceInfo?.Address))
				return;

			try
			{
				EnsureStartComplete();
				await RegisterInternalAsync(serviceInfo);
				_registerServiceDictionary.AddOrUpdate(serviceInfo, DateTime.Now, (key, oldValue) => DateTime.Now);
			}
			catch (KeeperException ex)
			{
				if (ex.getCode() == KeeperException.Code.SESSIONEXPIRED)
				{
					_startTask = StartAsync();
				}
				//else if (ex.getCode() == KeeperException.Code.CONNECTIONLOSS)
				//{
				//}
				throw;
			}
			catch (Exception ex)
			{
				LogHelper.Error(ex);
				throw;
			}
		}

		private async Task RegisterInternalAsync(ServiceInfo serviceInfo)
		{
			var appId = serviceInfo.Name;

			string appNodePath = GetAppNodePath(appId);
			var appNode = await _zookeeper.ExistsAsync(appNodePath).ConfigureAwait(false);
			if (appNode == null)
			{
				try
				{
					// ReSharper disable once UnusedVariable
					var appNodeName = await _zookeeper.CreateAsync(appNodePath, null, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT).ConfigureAwait(false);
				}
				catch (KeeperException ex)
				{
					if (ex.getCode() != KeeperException.Code.NODEEXISTS)
						throw;
				}
			}

			var envNodePath = appNodePath + "/" + (string.IsNullOrWhiteSpace(serviceInfo.Group) ? "_" : serviceInfo.Group);
			var envNode = await _zookeeper.ExistsAsync(envNodePath).ConfigureAwait(false);
			if (envNode == null)
			{
				try
				{
					// ReSharper disable once UnusedVariable
					var envNodeName = await _zookeeper.CreateAsync(envNodePath, null, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT).ConfigureAwait(false);
				}
				catch (KeeperException ex)
				{
					if (ex.getCode() != KeeperException.Code.NODEEXISTS)
						throw;
				}
			}

			var serviceNodePath = envNodePath + "/service-";
			// ReSharper disable once UnusedVariable
			var serviceNode = await _zookeeper.CreateAsync(serviceNodePath,
				serviceInfo.Address.GetBytes(), ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL_SEQUENTIAL).ConfigureAwait(false);
		}

		private static string GetAppNodePath(string appId)
		{
			return ServiceRootPath + "/" + appId;
		}

		public void Dispose()
		{
			if (_isDisposed) return;

			_isDisposed = true;
			_zookeeper?.CloseAsync().Wait();
		}

		public override Task process(WatchedEvent @event)
		{
			Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] Status: {@event.getState()}, Type: {@event.get_Type()}, Path: {@event.getPath()}");
			if (@event.getState() == Event.KeeperState.Expired)
			{
				if (_startTask?.IsCompleted == true)
					_startTask = StartAsync();
			}

			switch (@event.get_Type())
			{
				case Event.EventType.NodeChildrenChanged:
					break;
			}
			return Task.FromResult<object>(null);
		}

		private async Task StartAsync()
		{
			//if (_isStarted)
			//	return;

			if (_zookeeper != null)
				await _zookeeper.CloseAsync();

			_zookeeper = new Zookeeper(_registryAddress, _sessionTimeout, this);
			//_zookeeper = new Zookeeper("192.168.9.1:2181", 3600 * 1000, this);
			try
			{
				var root = await _zookeeper.ExistsAsync(ServiceRootPath).ConfigureAwait(false);

				if (root == null)
				{
					try
					{
						////创建一个节点root，数据是mydata,不进行ACL权限控制，节点为永久性的(即客户端shutdown了也不会消失) 
						// ReSharper disable once UnusedVariable
						var serviceRootNode = await _zookeeper.CreateAsync(ServiceRootPath, "rpclite service root node".GetBytes(),
							ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT).ConfigureAwait(false);
					}
					catch (KeeperException ex)
					{
						if (ex.getCode() != KeeperException.Code.NODEEXISTS)
							throw;
					}
				}

				if (!_registerServiceDictionary.IsEmpty)
				{
					foreach (var item in _registerServiceDictionary)
					{
						await RegisterInternalAsync(item.Key);
					}
				}

				if (!_serviceAddressDictionary.IsEmpty)
				{
					foreach (var item in _serviceAddressDictionary)
					{
						await LookupInternalAsync(new ServiceIdentifier(item.Value.ClientInfo.Name, item.Value.ClientInfo.Group));
					}
				}

				//_isStarted = true;
			}
			catch (KeeperException ex)
			{
				LogHelper.Error(ex);
				throw;
			}
			catch (Exception ex)
			{
				LogHelper.Error(ex);
				throw;
			}
		}

		internal class ClientLookupItem
		{
			public ServiceInfo[] Addresses { get; internal set; }
			public ServiceIdentifier ClientInfo { get; internal set; }
		}
	}
}
