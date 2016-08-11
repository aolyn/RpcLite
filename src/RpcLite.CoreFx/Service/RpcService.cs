using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RpcLite.Logging;

namespace RpcLite.Service
{
	/// <summary>
	/// Respresnts service infomation
	/// </summary>
	public class RpcService
	{
		private readonly ActionManager _actionManager;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		public RpcService(Type serviceType)
		{
			Type = serviceType;
			_actionManager = new ActionManager(serviceType);
		}

		#region properties

		/// <summary>
		/// Service request url path
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Service's Type
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Name of Service
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public List<IServiceFilter> Filters;

		/// <summary>
		/// 
		/// </summary>
		public event Action<ServiceContext> BeforeInvoke;

		/// <summary>
		/// 
		/// </summary>
		public event Action<ServiceContext> AfterInvoke;

		/// <summary>
		/// Convert to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name}, {Path}, {Type}";
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public Task ProcessRequestAsync(ServiceContext context)
		{
			//context.Service = this;
			LogHelper.Debug("RpcService.BeginProcessRequest");

			if (context.Request.ActionName == "?meta")
			{
				LogHelper.Debug("RpcService.BeginProcessRequest: start ActionHelper.InvokeAction");
				try
				{
					var metaInfo = GetMetaInfo(context.Service);
					context.Result = metaInfo;
					context.Request.RequestType = RequestType.MetaData;
				}
				catch (Exception ex)
				{
					context.Exception = ex;
				}
				LogHelper.Debug("RpcService.BeginProcessRequest: end ActionHelper.InvokeAction");

				return TaskHelper.FromResult((object)null);
			}

			LogHelper.Debug("RpcService.BeginProcessRequest: start ActionHelper.GetActionInfo");
			var action = _actionManager.GetAction(context.Request.ServiceType, context.Request.ActionName);
			LogHelper.Debug("RpcService.BeginProcessRequest: end ActionHelper.GetActionInfo");
			if (action == null)
			{
				LogHelper.Debug("Action Not Found: " + context.Request.ActionName);
				throw new ActionNotFoundException(context.Request.ActionName);
			}

			LogHelper.Debug("RpcService.BeginProcessRequest: got requestObject");

			context.Action = action;

			Func<ServiceContext, Task> filterFunc = ProcessRequest;
			if (Filters != null && Filters.Count > 0)
			{
				//TODO: confirm if Func will leak of memory
				var preFilterFunc = filterFunc;
				for (var idxFilter = 0; idxFilter < Filters.Count; idxFilter++)
				{
					var thisFilter = Filters[idxFilter];
					if (!thisFilter.FilterInvoke) continue;

					var nextFilterFunc = preFilterFunc;
					Func<ServiceContext, Task> currentFilterFunc = sc => thisFilter.Invoke(sc, nextFilterFunc);

					preFilterFunc = currentFilterFunc;
				}
				filterFunc = preFilterFunc;
			}

			return filterFunc(context);
		}

		private Task ProcessRequest(ServiceContext context)
		{
			try
			{
				BeforeInvoke?.Invoke(context);
			}
			catch (Exception ex)
			{
				LogHelper.Error(ex);
			}

			var task = context.Action.ExecuteAsync(context);
			var waitTask = task.ContinueWith(tsk =>
			{
				try
				{
					AfterInvoke?.Invoke(context);
				}
				catch (Exception ex)
				{
					LogHelper.Error(ex);
				}

				//var endDate = DateTime.Now;
				//var serviceDuration = endDate - startDate;

				//if (tsk.IsFaulted)
				//{
				//	context.Exception = tsk.Exception.InnerException;
				//}
				//else
				//{
				//	var result = RpcAction.GetResultObject(tsk, context);
				//	context.Result = result;
				//}
			});

			return waitTask;
		}

		private object GetMetaInfo(RpcService service)
		{
			var type = service.Type;
			var sb = new StringBuilder();
			sb.AppendFormat("Service Name: {0}", type.Name);
			sb.AppendLine();
			sb.Append("Actions:");

			//			var typeInfo =
			//#if NETCORE
			//				type.GetTypeInfo();
			//#else
			//				type;
			//#endif

			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

			foreach (var method in methods)
			{
				if (method.DeclaringType == typeof(object))
					continue;

				sb.AppendLine();
				sb.Append(method.ReturnType.Name);
				sb.Append(" ");
				sb.AppendFormat("{0}(", method.Name);

				bool isFirstArgument = true;
				foreach (var arg in method.GetParameters())
				{
					if (isFirstArgument)
						isFirstArgument = false;
					else
						sb.Append(", ");

					sb.Append(arg.ParameterType.Name);
					sb.Append(" ");
					sb.Append(arg.Name);
				}

				sb.AppendFormat(");", method.Name);

			}

			return sb.ToString();
		}
	}
}
