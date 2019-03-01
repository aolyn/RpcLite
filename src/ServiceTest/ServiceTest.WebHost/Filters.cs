using System;
using System.Diagnostics;
using System.Threading.Tasks;
using RpcLite.Filter;
using RpcLite.Service;

namespace ServiceTest.WebHost
{

	class LogTimeFilter : IProcessFilter
	{
		public string Name { get; set; } = nameof(LogTimeFilter);

		public async Task ProcessAsync(ServiceContext context, Func<ServiceContext, Task> next)
		{
			var stopwatch = Stopwatch.StartNew();
			await next(context);
			stopwatch.Stop();
			Console.WriteLine($"Service: {context.Service.Name}, Action: {context.Action.Name}, Execute Duration: {stopwatch.ElapsedMilliseconds}ms");
		}
	}

	class LogRequestTimeFilter : IProcessFilter
	{
		public string Name { get; set; } = nameof(LogRequestTimeFilter);

		public async Task ProcessAsync(ServiceContext context, Func<ServiceContext, Task> next)
		{
			//var stopwatch = Stopwatch.StartNew();
			await next(context);
			//stopwatch.Stop();
			//Console.WriteLine($"Service: {context.Service.Name}, Action: {context.Action.Name}, Request Length: {context.Request.ContentLength}bytes");
		}
	}

	class ResultFilter : IActionExecuteFilter
	{
		public string Name { get; set; } = nameof(ResultFilter);

		public void OnExecuted(ServiceContext context)
		{
			if (context.Exception != null)
			{
				Console.WriteLine(context.Action.Name + " exception occored: " + context.Exception);
			}
			else
			{
				Console.WriteLine(context.Action.Name + " execute result: " + Newtonsoft.Json.JsonConvert.SerializeObject(context.Result).Substring(0, 100));
			}
		}

		public void OnExecuting(ServiceContext context)
		{
			Console.WriteLine(context.Action.Name + " before execute action");
		}
	}

}
