using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RpcLite.Service;

namespace ServiceTest.WebHost
{

	class LogTimeFilter : IServiceFilter
	{
		public bool FilterInvoke { get; } = true;

		public string Name { get; set; }

		public void AfterInvoke(ServiceContext context)
		{
		}

		public void BeforeInvoke(ServiceContext context)
		{
		}

		public async Task Invoke(ServiceContext context, Func<ServiceContext, Task> next)
		{
			var stopwatch = Stopwatch.StartNew();
			await next(context);
			stopwatch.Stop();
			//Console.WriteLine($"Service: {context.Service.Name}, Action: {context.Action.Name}, Execute Duration: {stopwatch.ElapsedMilliseconds}ms");
		}
	}

	class LogRequestTimeFilter : IServiceFilter
	{
		public string Name { get; set; }

		public bool FilterInvoke { get; } = true;

		public void AfterInvoke(ServiceContext context)
		{
		}

		public void BeforeInvoke(ServiceContext context)
		{
		}

		public async Task Invoke(ServiceContext context, Func<ServiceContext, Task> next)
		{
			var stopwatch = Stopwatch.StartNew();
			await next(context);
			stopwatch.Stop();
			//Console.WriteLine($"Service: {context.Service.Name}, Action: {context.Action.Name}, Request Length: {context.Request.ContentLength}bytes");
		}

	}
}
