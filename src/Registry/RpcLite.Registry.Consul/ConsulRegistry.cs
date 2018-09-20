using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Consul;
using RpcLite.Config;

namespace RpcLite.Registry.Consul
{
	public class ConsulRegistry : IRegistry
	{
		private readonly ConcurrentDictionary<string, ConsulTtlChecker> _checkers
			= new ConcurrentDictionary<string, ConsulTtlChecker>();
		private readonly RpcConfig _config;
		private readonly ConsulAddressInfo _addressInfo;
		private readonly object _serviceDiscoveryLock = new object();

		private Dictionary<NameGroupPair, ServiceDiscovery> _serviceDiscoveries =
			new Dictionary<NameGroupPair, ServiceDiscovery>();

		public bool CanRegister => true;

		public ConsulRegistry(RpcConfig config)
		{
			_config = config;
			var address = _config.Registry.Address;
			_addressInfo = ParseServers(address);
		}

		public Task RegisterAsync(ServiceInfo serviceInfo)
		{
			var key = $"{serviceInfo.Name}::{serviceInfo.Group}";
			if (_checkers.ContainsKey(key))
				throw new ArgumentOutOfRangeException(nameof(serviceInfo), @"service already registered");

			var checker = new ConsulTtlChecker(serviceInfo.Name, serviceInfo.Group, serviceInfo.Address, _addressInfo);
			_checkers.TryAdd(key, checker);
			var _ = checker.Start();

			return Task.CompletedTask;
		}

		public Task<ServiceInfo[]> LookupAsync<TContract>()
		{
			var type = typeof(TContract);
			var clientConfigItem = _config.Client.Clients
				.First(it => it.TypeName == type.FullName);

			return LookupAsync(clientConfigItem.Name, clientConfigItem.Group);
		}

		public Task<ServiceInfo[]> LookupAsync(string name)
		{
			return LookupAsync(name, null);
		}

		public Task<ServiceInfo[]> LookupAsync(string name, string group)
		{
			var key = new NameGroupPair(name, group);
			if (_serviceDiscoveries.TryGetValue(key, out var serviceDiscovery))
			{
				return serviceDiscovery.LookupAsync();
			}

			lock (_serviceDiscoveryLock)
			{
				if (_serviceDiscoveries.TryGetValue(key, out serviceDiscovery))
				{
					return serviceDiscovery.LookupAsync();
				}

				var discovery = new ServiceDiscovery(name, group, _addressInfo);
				discovery.Start();
				var newDic = _serviceDiscoveries = new Dictionary<NameGroupPair, ServiceDiscovery>(_serviceDiscoveries);
				newDic[key] = discovery;
				_serviceDiscoveries = newDic;
				return discovery.LookupAsync();
			}

		}

		public void Dispose()
		{
			foreach (var checker in _checkers)
			{
				checker.Value.Stop();
			}
		}

		private static ConsulAddressInfo ParseServers(string address)
		{
			var info = new ConsulAddressInfo
			{
				Ttl = 10,
				CheckInterval = -1,
			};
			var uri = new Uri(address);
			string dc = null;
			var servers = new List<ConsulClientConfiguration>();
			if (uri.Query.Length > 7)
			{
				var segs = uri.Query.Substring(1).Split('&');
				var hosts = new Dictionary<string, string>();
				var ports = new Dictionary<string, string>();
				foreach (var item in segs)
				{
					var index = item.IndexOf('=');
					var value = item.Substring(index + 1);
					var key = item.Substring(0, index);

					if (key == "dc")
					{
						dc = value;
						continue;
					}

					if (key == "ttl")
					{
						info.Ttl = int.Parse(value);
						continue;
					}

					if (key == "interval")
					{
						info.CheckInterval = int.Parse(value);
						continue;
					}

					var hostMatch = Regex.Match(item, "host(\\d+)");
					if (hostMatch.Success)
					{
						hosts[hostMatch.Groups[1].Value] = value;
					}
					else
					{
						var portMatch = Regex.Match(item, "port(\\d+)");
						if (portMatch.Success)
						{
							ports[portMatch.Groups[1].Value] = value;
						}
					}
				}

				foreach (var host in hosts)
				{
					var port = ports.TryGetValue(host.Key, out var portString)
						? int.Parse(portString)
						: 8500;
					servers.Add(new ConsulClientConfiguration
					{
						Address = new Uri($"{uri.Scheme}://{host.Value}:{port}"),
						Datacenter = dc,
					});
				}
			}

			servers.Insert(0, new ConsulClientConfiguration
			{
				Address = new Uri($"{uri.Scheme}://{uri.Host}:{uri.Port}"),
				Datacenter = dc,
			});

			info.Servers = servers.ToArray();
			return info;
		}

		private struct NameGroupPair
		{
			// ReSharper disable NotAccessedField.Local
			public string Name;
			public string Group;
			// ReSharper restore NotAccessedField.Local

			public NameGroupPair(string name, string group)
			{
				Name = name;
				Group = group;
			}
		}
	}
}