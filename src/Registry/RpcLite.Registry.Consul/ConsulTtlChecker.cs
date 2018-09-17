using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Consul;

namespace RpcLite.Registry.Consul
{
	internal class ConsulTtlChecker
	{
		private readonly string _serviceName;
		private readonly ConsulAddressInfo _consulAddress;
		private readonly string _serviceId;
		private readonly string _address;

		private CancellationTokenSource _cancellationTokenSource;
		private int _serverIndex;

		public ConsulTtlChecker(string serviceName, string group, string address, ConsulAddressInfo consulAddress)
		{
			_serviceName = !string.IsNullOrEmpty(group)
				? serviceName + "@" + group
				: serviceName;
			_serviceId = $"{serviceName}@{group}::{Guid.NewGuid().ToString("n")}";
			_address = address;
			_consulAddress = consulAddress;
		}

		public async Task Start()
		{
			await RegisterAsync();
			_cancellationTokenSource = new CancellationTokenSource();
			var checkId = "service:" + _serviceId;

			var interval = _consulAddress.CheckInterval;
			if (interval == -1)
			{
				interval = _consulAddress.Ttl > 50
					? _consulAddress.Ttl - 10
					: (int)(_consulAddress.Ttl * 0.8);
			}

			var _ = CheckServiceAsync(checkId, interval, _cancellationTokenSource.Token);
		}

		private async Task RegisterAsync()
		{
			for (var i = 0; i < _consulAddress.Servers.Length; i++)
			{
				try
				{
					var server = GetConsulAddress();
					var client = GetClient(server);
					using (client)
					{
						var asr = new AgentServiceRegistration
						{
							ID = _serviceId,
							Name = _serviceName,
							Address = _address,
							Check = new AgentCheckRegistration
							{
								//DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5), //not work for ttl
								TTL = TimeSpan.FromSeconds(_consulAddress.Ttl),
							},
						};

						await client.Agent.ServiceRegister(asr).ConfigureAwait(false);
						break;
					}
				}
				catch (Exception)
				{
					_serverIndex++;
					_serverIndex %= _consulAddress.Servers.Length;
					await Task.Delay(100);
					//ignore
				}
			}
		}

		private async Task DeRegisterAsync(ConsulClientConfiguration server)
		{
			var client = GetClient(server);
			using (client)
			{
				await client.Agent.ServiceDeregister(_serviceId).ConfigureAwait(false);
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

		enum CheckStatus
		{
			Normal,
			NotRegister
		}

		private async Task CheckServiceAsync(string checkId, int interval, CancellationToken cancellationToken)
		{
			var status = CheckStatus.Normal;

			using (var client = GetClient(GetConsulAddress()))
			{
				while (true)
				{
					try
					{
						if (cancellationToken.IsCancellationRequested)
						{
							break;
						}

						var continueWhile = false;
						switch (status)
						{
							case CheckStatus.Normal:
								await client.Agent.PassTTL(checkId, null, cancellationToken);
								break;
							case CheckStatus.NotRegister:
								await RegisterAsync();
								status = CheckStatus.Normal;
								continueWhile = true;
								break;
						}

						if (continueWhile) continue;

						await Task.Delay(interval * 1000, cancellationToken);
					}
					catch (ConsulRequestException ex)
					{
						//Unexpected response, status code InternalServerError: CheckID "service:TimeService::::e7472714151849b491fd226bd70b9443" does not have associated TTL
						//CheckID "service:TimeService::::8be004bf39ee48388412e9da5be9ce70" does not have associated TTL
						//const string prefix = "CheckID ";
						const string subfix = " does not have associated TTL";
						if ( /*ex.Message.StartsWith(prefix) && */ex.Message.EndsWith(subfix))
						{
							status = CheckStatus.NotRegister;
						}

						Console.WriteLine(ex);
					}
					catch (HttpRequestException ex)
					{
						Console.WriteLine(ex);
					}
					catch (TaskCanceledException)
					{
						break;
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						throw;
					}
				}
			}
		}

		public void Stop()
		{
			DeRegisterAsync(GetConsulAddress()).GetAwaiter().GetResult();
		}

		private ConsulClientConfiguration GetConsulAddress()
		{
			return _consulAddress.Servers[_serverIndex];
		}
	}
}