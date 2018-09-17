using System;
using System.Collections.Concurrent;
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

		public bool CanRegister => true;

		public ConsulRegistry(RpcConfig config)
		{
			_config = config;
		}

		public Task RegisterAsync(ServiceInfo serviceInfo)
		{
			var key = $"{serviceInfo.Name}::{serviceInfo.Group}";
			if (_checkers.ContainsKey(key))
				throw new ArgumentOutOfRangeException(nameof(serviceInfo), @"service already registered");

			var address = _config.Registry.Address;
			var servers = new[]
			{
				new ConsulClientConfiguration
				{
					Address=new Uri(address)
				}
			};
			var checker = new ConsulTtlChecker(serviceInfo.Name, serviceInfo.Group, serviceInfo.Address, servers);
			_checkers.TryAdd(key, checker);
			var _ = checker.Start();

			return Task.CompletedTask;
		}

		public Task<ServiceInfo[]> LookupAsync<TContract>()
		{
			throw new NotImplementedException();
		}

		public Task<ServiceInfo[]> LookupAsync(string name)
		{
			throw new NotImplementedException();
		}

		public Task<ServiceInfo[]> LookupAsync(string name, string @group)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			foreach (var checker in _checkers)
			{
				checker.Value.Stop();
			}
		}
	}
}