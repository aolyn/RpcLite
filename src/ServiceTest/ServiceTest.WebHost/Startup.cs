using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RpcLite.Config;
using ServiceTest.ServiceImpl;

namespace ServiceTest.WebHost
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			//services.AddRouting();
			services.AddRpcLite(builder => builder
				.AddService<TestService>("TestService", "api/test/")
				.AddService<ProductService>("ProductService", "api/service/", null, ServiceLifecycle.Scoped)
				.AddService<TimeService>("TimeService", "api/time/")
				.AddFilter<TestFilterFactory>());

			services.AddConfigurationAssembly<Startup>();
			//services.AddAssembly<Startup>();
			//services.AddAutoConfiguration<ServiceConfiguration>();

			//var stopwatch = Stopwatch.StartNew();
			//var assemblies = GetAllAssemblies();
			//stopwatch.Stop();
			//Console.WriteLine($"load all assembly cost {stopwatch.ElapsedMilliseconds}ms");
			//stopwatch.Reset();
			//foreach (var item in assemblies)
			//{
			//	services.AddAssembly(item);
			//}
			//Console.WriteLine($"register all assembly cost {stopwatch.ElapsedMilliseconds}ms");
		}

		///// <summary>
		///// 获取项目程序集，排除所有的系统程序集(Microsoft.***、System.***等)、Nuget下载包
		///// </summary>
		///// <returns></returns>
		//public static IList<Assembly> GetAllAssemblies()
		//{
		//	var list = new List<Assembly>();
		//	var deps = DependencyContext.Default;
		//	var libs = deps.CompileLibraries
		//		.Where(lib => !lib.Serviceable /*&& lib.Type != "package"*/)
		//		.ToArray();//排除所有的系统程序集、Nuget下载包
		//	foreach (var lib in libs)
		//	{
		//		try
		//		{
		//			var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
		//			list.Add(assembly);
		//		}
		//		catch (Exception)
		//		{
		//			// ignored
		//		}
		//	}
		//	return list;
		//}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(LogLevel.Error);

			//RpcManager.AddFilter(new LogTimeFilter());
			//RpcManager.AddFilter(new LogRequestTimeFilter());

			//Method 3: use builder
			app.UseRpcLite();
			//app.UseRpcLite(builder => builder
			//	.UseService<TestService>("TestService", "api/test/")
			//	.UseService<ProductService>("ProductService", "api/service/")
			//	.UseFilter<TestFilterFactory>());

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");

				////Method2: use IRouteBuilder
				//routes.UseRpcLite(builder => builder
				//	.UseService<TestService>("TestService", "api/test/")
				//	.UseService<ProductService>("TestService", "api/service/")
				//	.UseFilter<TestFilterFactory>());
			});

			app.Run(async (context) =>
			{
				await context.Response.WriteAsync("Hello World!");
			});
		}
	}
}