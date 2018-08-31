using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ServiceTest.Common;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceConfigurationExtension
	{
		public static IServiceCollection AddServiceConfiguration<T>(this IServiceCollection services)
		{
			return AddAutoConfiguration(services, typeof(T));
		}

		public static IServiceCollection AddAutoConfiguration(this IServiceCollection services, Type type)
		{
			var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
			foreach (var method in methods)
			{
				var attr = method.GetCustomAttributes<ServiceAttribute>().FirstOrDefault();
				if (attr == null)
					return services;

				var para = Expression.Parameter(typeof(IServiceProvider), "services");

				//var varExp = Expression.Variable(typeof(object), "result");

				var invokeExp = Expression.Call(method, para);
				var convertExp = Expression.Convert(invokeExp, typeof(object));
				//var assignExp = Expression.Assign(varExp, convertExp);

				var labelTarget = Expression.Label(typeof(object));
				var returnExp = Expression.Return(labelTarget, convertExp);
				var lbl = Expression.Label(labelTarget, Expression.Constant(null));

				var blocks = Expression.Block(/*new[] { varExp }, assignExp,*/ returnExp, lbl);

				var lam = Expression.Lambda<Func<IServiceProvider, object>>(blocks, para);
				var func = lam.Compile();

				var desc = new ServiceDescriptor(method.ReturnType, func, attr.Lifetime);
				services.Add(desc);
			}
			return services;
		}
	}
}