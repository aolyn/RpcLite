using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RpcLite.Config;
using RpcLite.Monitor.Contract;
using RpcLite.Service;

namespace RpcLite.Monitor.Merops
{
	public class MeropsMonitor : IMonitor
	{
		private readonly IMonitorService _client;
		private List<InvokeInfo> _invokes = new List<InvokeInfo>();
		private readonly object _logLock = new object();
		private bool _disposed;

		public MeropsMonitor(AppHost appHost, RpcConfig config)
		{
			//var factory = new RpcClientFactory(null, null);
			_client = appHost.ClientFactory.GetInstance<IMonitorService>(config?.Monitor.Address);
			// ReSharper disable once UnusedVariable
			var writeTask = WriteLogsAsync();
		}

		public IServiceMonitorSession GetServiceSession(ServiceContext context)
		{
			var session = new MeropsMonitorSession();
			session.OnEnd += Session_OnEnd;
			return session;
		}

		public IClientMonitorSession GetClientSession()
		{
			return null;
		}

		private void Session_OnEnd(object sender, ServiceContext e)
		{
			lock (_logLock)
			{
				_invokes.Add(((MeropsMonitorSession)sender).InvokeInfo);
			}
		}

		private async Task WriteLogsAsync()
		{
			while (!_disposed)
			{
				List<InvokeInfo> toWriteInvokes = null;
				lock (_logLock)
				{
					if (_invokes.Count > 0)
					{
						toWriteInvokes = _invokes;
						_invokes = new List<InvokeInfo>();
					}
				}

				if (toWriteInvokes != null)
				{
					var pageSize = 1000;
					var pageCount = (toWriteInvokes.Count - 1) / pageSize + 1;
					for (var idxPage = 0; idxPage < pageCount; idxPage++)
					{
						var currentPageItems = toWriteInvokes
							.Skip(idxPage * pageSize)
							.Take(pageSize)
							.ToArray();

						for (var i = 0; i < 3; i++)
						{
							try
							{
								var writeTask = _client.AddInvokesAsync(currentPageItems);
								await writeTask;
								break;
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex);
							}
						}
					}

				}

				await Task.Delay(10 * 1000);
			}
		}

		public void Dispose()
		{
			_disposed = true;
		}
	}

	internal class MeropsMonitorSession : IServiceMonitorSession
	{
		//private IMonitor _monitor;
		private readonly InvokeInfo _info = new InvokeInfo();
		public event EventHandler<ServiceContext> OnEnd;

		//public MeropsMonitorSession(IMonitor monitor)
		//{
		//	_monitor = monitor;
		//}

		public InvokeInfo InvokeInfo => _info;

		public void BeginRequest(ServiceContext context)
		{
			_info.Id = Guid.NewGuid().ToString();
			_info.Service = context.Service.Name;
			_info.StartDate = DateTime.UtcNow;
		}

		public void EndRequest(ServiceContext context)
		{
			_info.Action = context.Action.Name;
			_info.EndDate = DateTime.UtcNow;
			_info.Duration = (int)(_info.EndDate - _info.StartDate).TotalMilliseconds;
			OnEnd?.Invoke(this, context);
		}

	}

}
