using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RpcLite.Service;

namespace ServiceTest.ClientTest
{

	class LogTimeFilter : IProcessFilter
	{
		public string Name { get; set; } = nameof(LogTimeFilter);

		public async Task ProcessAsync(ServiceContext context, Func<ServiceContext, Task> next)
		{
			var stopwatch = Stopwatch.StartNew();
			await next(context);
			stopwatch.Stop();
			//Console.WriteLine($"Service: {context.Service.Name}, Action: {context.Action.Name}, Execute Duration: {stopwatch.ElapsedMilliseconds}ms");
		}
	}

	class LogRequestTimeFilter : IProcessFilter
	{
		public string Name { get; set; } = nameof(LogRequestTimeFilter);

		public async Task ProcessAsync(ServiceContext context, Func<ServiceContext, Task> next)
		{
			var stopwatch = Stopwatch.StartNew();
			await next(context);
			stopwatch.Stop();
			//Console.WriteLine($"Service: {context.Service.Name}, Action: {context.Action.Name}, Request Length: {context.Request.ContentLength}bytes");
		}
	}

	class EmptyFilter : IProcessFilter
	{
		public string Name { get; set; } = nameof(EmptyFilter);

		public async Task ProcessAsync(ServiceContext context, Func<ServiceContext, Task> next)
		{
			await next(context);
		}
	}

}
