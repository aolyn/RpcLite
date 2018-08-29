using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RpcLite.Config;

namespace RpcLite.Server.Kestrel
{
	public class StartupBuilder
	{
		private static readonly ModuleBuilder ModuleBuilder;
		private static int _startupIndex;

		static StartupBuilder()
		{
			const string assemblyName = "RpcLiteStartupBuilderAssembly";
			var asmName = new AssemblyName { Name = assemblyName };
			var asmFileName = asmName.Name + ".dll";

			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndCollect);
			ModuleBuilder = assemblyBuilder.DefineDynamicModule(asmFileName);
		}

		public static Type Create(Action<RpcConfigBuilder> configServices)
		{
			var typeIndex = Interlocked.Increment(ref _startupIndex);
			var typeBuilder = ModuleBuilder.DefineType("Startup_" + typeIndex);
			var field1 = typeBuilder.DefineField("ConfigBuilder", typeof(Action<RpcConfigBuilder>),
				FieldAttributes.Public | FieldAttributes.Static);

			var configServicesMethod = typeBuilder.DefineMethod("ConfigureServices", MethodAttributes.Public,
				typeof(void), new[] { typeof(IServiceCollection) });
			var body1 = configServicesMethod.GetILGenerator();
			body1.Emit(OpCodes.Ldarg_1);
			body1.Emit(OpCodes.Ldsfld, field1);
			var configureServices = typeof(StartupBuilder).GetMethod("ConfigureServices",
				BindingFlags.Static | BindingFlags.Public);
			body1.Emit(OpCodes.Call, configureServices);
			body1.Emit(OpCodes.Ret);

			var configureMethod = typeBuilder.DefineMethod("Configure", MethodAttributes.Public,
				typeof(void), new[] { typeof(IApplicationBuilder), typeof(IHostingEnvironment), typeof(ILoggerFactory) });
			var il2 = configureMethod.GetILGenerator();
			il2.Emit(OpCodes.Ldarg_1);
			//il2.Emit(OpCodes.Ldsfld, field1);
			var sConfig = typeof(StartupBuilder).GetMethod("Configure", BindingFlags.Static | BindingFlags.Public);
			il2.Emit(OpCodes.Call, sConfig);
			il2.Emit(OpCodes.Ret);

			var startupType = typeBuilder.CreateTypeInfo().AsType();
			var field2 = startupType.GetField("ConfigBuilder", BindingFlags.Static | BindingFlags.Public);
			field2?.SetValue(null, configServices);

			return startupType;
		}

		[Obsolete("for emit use only")]
		public static void ConfigureServices(IServiceCollection services, Action<RpcConfigBuilder> configBuilder)
		{
			services.AddRouting();
			services.AddRpcLite(configBuilder);
		}

		[Obsolete("for emit use only")]
		public static void Configure(IApplicationBuilder app)
		{
			app.UseRpcLite();

			app.Run(async context =>
			{
				await context.Response.WriteAsync("RpcLite Server is running");
			});
		}
	}
}
