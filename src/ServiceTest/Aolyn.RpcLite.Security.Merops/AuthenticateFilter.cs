using System;
using System.Linq;
using System.Threading.Tasks;
using Aolyn.RpcLite.Security.Abstracts;
using RpcLite;
using RpcLite.Filter;
using RpcLite.Service;

namespace Aolyn.RpcLite.Security.Merops
{
	public class AuthenticateFilter : IActionExecutingFilter
	{
		private readonly CopyOnWriteDictionary<RpcAction, object> _actionAuthConfigs
			= new CopyOnWriteDictionary<RpcAction, object>();

		public string Name { get; set; } = nameof(AuthenticateFilter);

		public Task ProcessAsync(ServiceContext context, Func<ServiceContext, Task> next)
		{
			var processor = _actionAuthConfigs.GetOrAdd(context.Action, key =>
			 {
				 var attribute = context.Action.MethodInfo.CustomAttributes
					.FirstOrDefault(it => it.AttributeType == typeof(AuthorizeAttribute));
				 var p = attribute == null ? null : new object();
				 return p;
			 });

			if (processor != null)
			{
				//TODO: do auth check
			}

			//context.Service
			Console.WriteLine(context.Action.Name + " exception occored: " + context.Exception);
			return next(context);
		}
	}
}
