using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Consul;

namespace RpcLite.Registry.Consul
{
	internal class ServiceDiscovery
	{
		private readonly string _serviceName;

		private readonly ConsulAddressInfo _consulAddress;

		private CancellationTokenSource _cancellationTokenSource;
		//private int _serverIndex;
		private ulong _lastIndex;
		private ServiceInfo[] _serviceAddress;

		public ServiceDiscovery(string serviceName, string group, ConsulAddressInfo consulAddress)
		{
			_serviceName = !string.IsNullOrEmpty(group)
				? serviceName + "@" + group
				: serviceName;
			_consulAddress = consulAddress;
		}

		public void Start()
		{
			_cancellationTokenSource = new CancellationTokenSource();
			var _ = WatchAsync(_cancellationTokenSource.Token);
		}

		public async Task<ServiceInfo[]> LookupAsync()
		{
			if (_serviceAddress != null)
				return _serviceAddress;

			QueryResult<CatalogService[]> queryResult = null;
			var serverIndex = 0;
			for (var j = 0; j < _consulAddress.Servers.Length; j++)
			{
				try
				{
					var server = _consulAddress.Servers[serverIndex];
					using (var consulClient = GetClient(server))
					{
						queryResult = await consulClient.Catalog.Service(_serviceName);
						break;
					}
				}
				catch (Exception)
				{
					serverIndex++;
					serverIndex %= _consulAddress.Servers.Length;
					await Task.Delay(50);
				}
			}

			if (_serviceAddress != null)
				return _serviceAddress;

			if (queryResult == null)
				return new ServiceInfo[0];

			return queryResult.Response
				.Select(it =>
				{
					var nameSplitIndex = it.ServiceName.IndexOf('@');
					var name = nameSplitIndex > -1
						? it.ServiceName.Substring(0, nameSplitIndex)
						: it.ServiceName;
					var group = nameSplitIndex > -1
						? it.ServiceName.Substring(nameSplitIndex + 1)
						: null;

					var address = it.ServiceAddress;
					return new ServiceInfo
					{
						Name = name,
						Group = group,
						Address = address,
					};
				})
				.ToArray();
		}

		public async Task WatchAsync(CancellationToken cancellationToken)
		{
			var serverIndex = 0;
			var consulClient = GetClient(_consulAddress.Servers[serverIndex]);
			while (true)
			{
				if (cancellationToken.IsCancellationRequested) break;

				try
				{
					var queryResult = await consulClient.Catalog.Service(_serviceName, null,
						 new QueryOptions { WaitIndex = _lastIndex }, cancellationToken);

					if (queryResult.LastIndex != _lastIndex)
					{
						_serviceAddress = queryResult.Response
							.Select(it =>
							{
								var nameSplitIndex = it.ServiceName.IndexOf('@');
								var name = nameSplitIndex > -1
									? it.ServiceName.Substring(0, nameSplitIndex)
									: it.ServiceName;
								var group = nameSplitIndex > -1
									? it.ServiceName.Substring(nameSplitIndex + 1)
									: null;

								var address = it.ServiceAddress;
								return new ServiceInfo
								{
									Name = name,
									Group = group,
									Address = address,
								};
							})
							.ToArray();

						_lastIndex = queryResult.LastIndex;
					}
				}
				catch (HttpRequestException)
				{
					serverIndex++;
					serverIndex %= _consulAddress.Servers.Length;

					consulClient.Dispose();
					consulClient = GetClient(_consulAddress.Servers[serverIndex]);
					await Task.Delay(50, cancellationToken);
				}
				catch (Exception)
				{
					await Task.Delay(50, cancellationToken);
				}
			}
		}

		private static ConsulClient GetClient(ConsulClientConfiguration server)
		{
			return new ConsulClient(config =>
			{
				config.Address = server.Address;
				config.Datacenter = server.Datacenter;
			});
		}
	}
}