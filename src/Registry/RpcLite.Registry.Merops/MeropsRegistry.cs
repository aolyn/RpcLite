using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpcLite.Client;
using RpcLite.Config;
using RpcLite.Logging;
using RpcLite.Registry.Merops.Contract;

namespace RpcLite.Registry.Merops
{
	/// <summary>
	/// simple registry, update service info from registry service periodicity by UpdateInterval
	/// </summary>
	public class MeropsRegistry : RegistryBase
	{
		private readonly object _initializeLocker = new object();
		private QuickReadConcurrentDictionary<ServiceIdentifier, ServiceInfo[]> _defaultBaseUrlDictionary = new QuickReadConcurrentDictionary<ServiceIdentifier, ServiceInfo[]>();
		private readonly ConcurrentDictionary<ServiceIdentifier, DateTime> _updateTimeDic = new ConcurrentDictionary<ServiceIdentifier, DateTime>();
		private readonly Lazy<IRegistryService> _registryClient;
		private readonly AppHost _appHost;
		private int _updateInterval = 3 * 60;

		/// <summary>
		/// update interval in seconds
		/// </summary>
		public int UpdateInterval
		{
			get { return _updateInterval; }
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException(nameof(value), "UpdateInterval must greater than 0");
				_updateInterval = value;
			}
		}

		public MeropsRegistry(AppHost appHost, RpcConfig config)
			: base(config)
		{
			Config = config;
			_appHost = appHost;

			_registryClient = new Lazy<IRegistryService>(() =>
			{
				var address = Config?.Registry?.Address;

				if (string.IsNullOrWhiteSpace(address))
				{
					LogHelper.Error("Registry Client Config Error: not exist or path is empty");
					return null;
				}

				var client = _appHost == null
					? ClientFactory.GetInstance<IRegistryService>(address)
					: _appHost.ClientFactory.GetInstance<IRegistryService>(address);
				return client;
			});

			InitilizeAddresses();
			// ReSharper disable once VirtualMemberCallInConstructor
			StartUpdateRegistry();
		}

		protected virtual void StartUpdateRegistry()
		{
			Action doUpdate = null;

			doUpdate = () =>
			{
				//after 3 minutes update again
				TaskHelper.Delay(15 * 1000).ContinueWith(tsk =>
				{
					try
					{
						DoUpdate();
					}
					catch (Exception ex)
					{
						LogHelper.Error(ex);
					}

					// ReSharper disable once AccessToModifiedClosure
					// ReSharper disable once PossibleNullReferenceException
					doUpdate();
				});
			};
			doUpdate();
		}

		private void DoUpdate()
		{
			var toUpdateServices = _updateTimeDic
				.Where(it => (DateTime.Now - it.Value).TotalSeconds > UpdateInterval)
				.Select(it => it.Key)
				.ToArray();

			var services = toUpdateServices
				.Select(it => new ServiceIdentifierDto
				{
					Name = it.Name,
					Group = it.Group
				})
				.ToArray();

			if (services.Length == 0) return;

			var request = new GetServiceInfoRequest { Services = services };

			try
			{
				var response = _registryClient.Value.GetServiceInfo(request);
				foreach (var item in response.Services)
				{
					var serviceInfos = item.ServiceInfos
						?.Select(it => new ServiceInfo
						{
							Name = it.Name,
							Group = it.Group,
							Address = it.Address,
							Data = it.Data,
						})
						.ToArray();

					var key = new ServiceIdentifier(item.Identifier.Name, item.Identifier.Group);
					_defaultBaseUrlDictionary[key] = serviceInfos;
					_updateTimeDic.AddOrUpdate(key, DateTime.Now, (k, oldValue) => DateTime.Now);
				}
			}
			catch (Exception ex)
			{
				LogHelper.Error(ex);
			}
		}

		private void InitilizeAddresses()
		{
			lock (_initializeLocker)
			{
				InitilizeBaseUrls();
			}
		}

		private void InitilizeBaseUrls()
		{
			var tempDic = GetAddresses<Dictionary<ServiceIdentifier, ServiceInfo[]>>(Config)
				.Where(it => it.Value?.Select(v => v.Address).FirstOrDefault() != null)
				.ToDictionary(it => it.Key, it => it.Value);

			var tmp = new QuickReadConcurrentDictionary<ServiceIdentifier, ServiceInfo[]>();
			foreach (var item in tempDic)
			{
				tmp.Add(item.Key, item.Value);
			}

			_defaultBaseUrlDictionary = tmp;
		}

		public override bool CanRegister => false;

		private ServiceInfo[] GetAddressInternal(string name, string group)
		{
			var itemKey = new ServiceIdentifier(name, group);
			// ReSharper disable once InconsistentlySynchronizedField
			var url = _defaultBaseUrlDictionary.GetOrAdd(itemKey, key =>
			{
				if (key == ServiceIdentifier.Empty) return null;
				if (_registryClient.Value == null) return null;

				var request = new GetServiceInfoRequest
				{
					Services = new[]
					{
						new ServiceIdentifierDto
						{
							Name = name,
							Group = group,
						}
					}
				};
				try
				{
					var response = _registryClient.Value.GetServiceInfo(request);
					var result = response.Services.FirstOrDefault();
					var serviceInfos = result?.ServiceInfos
						.Select(it => new ServiceInfo
						{
							Name = name,
							Group = group,
							Address = it.Address,
							Data = it.Data,
						})
						.ToArray();

					_updateTimeDic.AddOrUpdate(itemKey, DateTime.Now, (k, oldValue) => DateTime.Now);

					return serviceInfos;
				}
				catch (Exception ex)
				{
					LogHelper.Error(ex);
					throw;
				}
			});

			return url;
		}

		public override Task RegisterAsync(ServiceInfo serviceInfo)
		{
			return TaskHelper.FromResult<object>(null);
		}

		public override Task<ServiceInfo[]> LookupAsync(string name, string group)
		{
#if NETCORE
			return Task.FromResult(GetAddressInternal(name, group));
#else
			return RpcLite.TaskHelper.FromResult(GetAddressInternal(name, group));
#endif
		}
	}
}