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
		private readonly ConsulClientConfiguration[] _servers;
		private readonly string _serviceId;
		private readonly string _address;
		private CancellationTokenSource _cancellationTokenSource;

		public ConsulTtlChecker(string serviceName, string group, string address, ConsulClientConfiguration[] servers)
		{
			_serviceName = serviceName;
			_serviceId = $"{serviceName}::{group}::{Guid.NewGuid().ToString("n")}";
			_servers = servers;
			_address = address;
		}

		public async Task Start()
		{
			var server = _servers[0];
			await RegisterAsync(server);
			_cancellationTokenSource = new CancellationTokenSource();
			var checkId = "service:" + _serviceId;
			var _ = CheckServiceAsync(checkId, 15, _cancellationTokenSource.Token);
		}

		private async Task RegisterAsync(ConsulClientConfiguration server)
		{
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
						DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
						TTL = TimeSpan.FromSeconds(20),
					},
				};

				await client.Agent.ServiceRegister(asr).ConfigureAwait(false);
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

			using (var client = GetClient(_servers[0]))
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
								var server = _servers[0];
								await RegisterAsync(server);
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
						const string prefix = "CheckID ";
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
			DeRegisterAsync(_servers[0]).GetAwaiter().GetResult();
		}
	}
}