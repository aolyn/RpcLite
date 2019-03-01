using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using RpcLite.Formatters;
using RpcLite.Logging;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcAction
	{
		private long _processFilterOldVersion;

		#region public properties

		/// <summary>
		/// 
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// 
		/// </summary>
		public MethodInfo MethodInfo { get; internal set; }

		/// <summary>
		/// 
		/// </summary>
		public int ArgumentCount { get; internal set; }

		/// <summary>
		/// 
		/// </summary>
		public Type ArgumentType { get; internal set; }

		/// <summary>
		/// 
		/// </summary>
		public bool HasReturnValue { get; internal set; }


		/// <summary>
		/// T: service, argument, callback, state, return
		/// </summary>
		internal Func<object, object, object> InvokeTask { get; set; }


		/// <summary>
		/// T: service, argument, return
		/// </summary>
		internal Func<object, object, object> Func { get; set; }

		/// <summary>
		/// 
		/// </summary>
		internal Action<object, object> Action { get; set; }

		/// <summary>
		/// 
		/// </summary>
		internal Func<object> ServiceCreator { get; set; }

		///// <summary>
		///// 
		///// </summary>
		//public bool IsAsync { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsStatic { get; internal set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsTask { get; internal set; }

		/// <summary>
		/// 
		/// </summary>
		public Type TaskResultType { get; internal set; }

		/// <summary>
		/// default value or argument type
		/// </summary>
		public object DefaultArgument { get; internal set; }

		/// <summary>
		/// 
		/// </summary>
		public RpcService Service { get; internal set; }

		/// <summary>
		/// result type of action, if return type of action is Task of T, it's T
		/// </summary>
		public Type ResultType { get; internal set; }

		#endregion

		/// <summary>
		/// invoke methods
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal Task ExecuteAsync(ServiceContext context)
		{
			return GetExecutingFilterFunc()(context);
		}

		private Task ExecuteInternalAsync(ServiceContext context)
		{
			if (ArgumentCount > 0)
			{
				try
				{
					context.Argument = context.Request.ContentLength > 0
						? GetRequestObject(context.Request.RequestStream, context.Formatter, ArgumentType)
						: DefaultArgument;
				}
				catch (Exception ex)
				{
					context.Exception = new DeserializeRequestException("deserialize request error", ex);
					return TaskHelper.FromResult<object>(null);
				}
			}

			ApplyExecutingFilters(context);

			if (IsTask)
			{
				try
				{
					var task = InvokeTaskInternal(context);

					var waitTask = task.ContinueWith(tsk =>
					{
						if (tsk.Exception != null)
						{
							context.Exception = tsk.Exception.InnerException;
						}
						else
						{
							var result = GetResultObject(tsk, context);
							context.Result = result;
						}

						ApplyExecutedFilters(context);
					});

					return waitTask;
				}
				catch (Exception ex)
				{
					context.Exception = ex;
					ApplyExecutedFilters(context);

					return TaskHelper.FromResult<object>(null);
				}
			}
			else
			{
				LogHelper.Debug("RpcService.BeginProcessRequest: start ActionHelper.InvokeAction");
				try
				{
					context.Result = InvokeAction(context);
				}
				catch (Exception ex)
				{
					context.Exception = ex;

					var tcs = new TaskCompletionSource<object>();
					tcs.SetResult(null);
					return tcs.Task;
				}
				ApplyExecutedFilters(context);
				LogHelper.Debug("RpcService.BeginProcessRequest: end ActionHelper.InvokeAction");

				return TaskHelper.FromResult(context.Result);
			}
		}

		private Func<ServiceContext, Task> _filterFunc;
		private Func<ServiceContext, Task> GetExecutingFilterFunc()
		{
			var filters = Service.ActionExecutingFilter;
			if (filters == null || filters.Count == 0)
				return ExecuteInternalAsync;

			var version = filters.Version;
			if (_processFilterOldVersion == version)
				return _filterFunc;

			//use copy on write instead of lock
			Func<ServiceContext, Task> filterFunc = ExecuteInternalAsync;
			var preFilterFunc = filterFunc;
			for (var idxFilter = filters.Count - 1; idxFilter > -1; idxFilter--)
			{
				var thisFilter = filters[idxFilter];

				var nextFilterFunc = preFilterFunc;
				//all currentFilterFunc.GetHashCode is the same
				Func<ServiceContext, Task> currentFilterFunc = sc => thisFilter.ProcessAsync(sc, nextFilterFunc);

				preFilterFunc = currentFilterFunc;
			}
			filterFunc = preFilterFunc;

			_filterFunc = filterFunc;
			_processFilterOldVersion = version;
			return filterFunc;
		}

		private void ApplyExecutingFilters(ServiceContext context)
		{
			if (!(Service?.ActionExecuteFilter?.Count > 0))
				return;

			foreach (var item in Service.ActionExecuteFilter)
			{
				try
				{
					item.OnExecuting(context);
				}
				catch (Exception ex)
				{
					LogHelper.Error(ex);
				}
			}
		}

		private void ApplyExecutedFilters(ServiceContext context)
		{
			if (!(Service?.ActionExecuteFilter?.Count > 0))
				return;

			foreach (var item in Service.ActionExecuteFilter)
			{
				try
				{
					item.OnExecuted(context);
				}
				catch (Exception ex)
				{
					LogHelper.Error(ex);
				}
			}
		}

		private static object GetRequestObject(Stream stream, IFormatter formatter, Type type)
		{
			try
			{
				return formatter.Deserialize(stream, type);
			}
			catch (Exception)
			{
				throw new RequestException("Parse request content to object error.");
			}
		}

		private static readonly QuickReadConcurrentDictionary<Type, Func<object, object>> GetTaskResultFuncs = new QuickReadConcurrentDictionary<Type, Func<object, object>>();

		/// <summary>
		/// get result of task
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		private static object GetTaskResult(Task task)
		{
			try
			{
				var type = task.GetType();
				var func = GetTaskResultFuncs.GetOrAdd(type, key =>
				{
					var arg = Expression.Parameter(typeof(object), "task");
					var argConvert = Expression.Convert(arg, type);
					var expProp = Expression.Property(argConvert, "Result");
					var expConvert = Expression.Convert(expProp, typeof(object));
					var asignExp = Expression.Lambda(expConvert, arg);
					var getFunc = asignExp.Compile();
					return (Func<object, object>)getFunc;
				});

				var value = func?.Invoke(task);
				return value;
			}
			catch (Exception ex)
			{
				LogHelper.Error("GetTaskResult error", ex);
				var aex = ex as AggregateException;
				if (aex?.InnerException != null)
					throw aex.InnerException;
				throw;
			}
		}

		private static object GetResultObject(IAsyncResult ar, ServiceContext context)
		{
			object resultObject = null;
			var serviceContainer = (ServiceInstanceContainer)context.ServiceContainer;

			try
			{
				if (context.Action.IsTask)
				{
					var task = ar as Task;
					if (task == null)
						throw new ServiceException("task async api return no Task result");

					if (context.Action.ResultType == null)
						return null;

					resultObject = GetTaskResult(task);
				}
				else
				{
					if (context.Action.HasReturnValue)
					{
						resultObject = context.Result;
					}
				}
			}
			finally
			{
				if (serviceContainer != null && !context.Action.IsStatic)
				{
					serviceContainer.Dispose();
				}
			}

			return resultObject;
		}

		private object GetServiceInstance(ServiceContext context)
		{
#if NETCORE
			if (context.RequestServices != null)
			{
				try
				{
					return context.RequestServices.GetService(MethodInfo.DeclaringType);
				}
				catch (Exception ex)
				{
					var message = "Resolve service instance failed: " + ex.Message;
					LogHelper.Error(message);
					throw new ResolveServiceInstanceException(message, ex);
				}
			}
#endif

			using (var serviceInstance = ServiceFactory.GetService(this))
			{
				return serviceInstance.ServiceObject;
			}
		}

		private object InvokeAction(ServiceContext context)
		{
			if (IsStatic)
			{
				return InvokeAction(context.Argument, null);
			}

			var serviceInstance = GetServiceInstance(context);
			return InvokeAction(context.Argument, serviceInstance);
		}

		private object InvokeAction(object requestObject, object serviceObject)
		{
			object resultObj;
			if (HasReturnValue)
			{
				resultObj = Func(serviceObject, requestObject);
			}
			else
			{
				Action(serviceObject, requestObject);
				resultObj = null;
			}
			return resultObj;
		}

		private Task InvokeTaskInternal(ServiceContext context)
		{
			var serviceObject = context.Action.IsStatic
				? null
				: GetServiceInstance(context);

			var ar = (Task)context.Action.InvokeTask(serviceObject, context.Argument);
			return ar;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Name;
		}

	}
}