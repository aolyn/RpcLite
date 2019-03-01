using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aolyn.RpcLite.Security.Abstracts;
using RpcLite.Filter;
using RpcLite.Service;

namespace Aolyn.RpcLite.Security.Merops
{
	public class AuthenticateFilter : IActionExecutingFilter
	{
		private readonly object _processLock = new object();
		private Dictionary<RpcAction, object> _actionAuthProcessors = new Dictionary<RpcAction, object>();

		public string Name { get; set; } = nameof(AuthenticateFilter);

		public Task ProcessAsync(ServiceContext context, Func<ServiceContext, Task> next)
		{
			if (!_actionAuthProcessors.TryGetValue(context.Action, out var processor))
			{
				lock (_processLock)
				{
					if (!_actionAuthProcessors.TryGetValue(context.Action, out processor))
					{
						var attribute = context.Action.MethodInfo.CustomAttributes
							.FirstOrDefault(it => it.AttributeType == typeof(AuthorizeAttribute));
						processor = attribute == null ? null : new object();
						_actionAuthProcessors.Add(context.Action, processor);
					}
				}
			}

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
