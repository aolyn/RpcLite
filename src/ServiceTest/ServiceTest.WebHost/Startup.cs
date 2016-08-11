using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RpcLite.Config;
using RpcLite.Service;

namespace ServiceTest.WebHost
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(LogLevel.Error);

			RpcLiteInitializer.Initialize(app);
			RpcProcessor.AddFilter(new LogTimeFilter());
			RpcProcessor.AddFilter(new LogRequestTimeFilter());

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.Run(async (context) =>
			{
				await context.Response.WriteAsync("Hello World!");
			});
		}
	}

	class LogTimeFilter : IServiceFilter
	{
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
			Console.WriteLine($"Service: {context.Service.Name}, Action: {context.Action.Name}, Execute Duration: {stopwatch.ElapsedMilliseconds}ms");
		}
	}

	class LogRequestTimeFilter : IServiceFilter
	{
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
			Console.WriteLine($"Service: {context.Service.Name}, Action: {context.Action.Name}, Request Length: {context.Request.ContentLength}bytes");
		}
	}


}
