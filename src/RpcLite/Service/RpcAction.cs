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
		#region public properties

		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public MethodInfo MethodInfo { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int ArgumentCount { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Type ArgumentType { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool HasReturnValue { get; set; }


		/// <summary>
		/// T: service, argument, callback, state, return
		/// </summary>
		public Func<object, object, object> InvokeTask { get; set; }


		/// <summary>
		/// T: service, argument, return
		/// </summary>
		public Func<object, object, object> Func { get; set; }

		///// <summary>
		///// T: service, argument, callback, state, return
		///// </summary>
		//public Func<object, object, AsyncCallback, object, IAsyncResult> BeginFunc { get; set; }

		////public Func<object, object, object, object, object> BeginFunc { get; set; }

		///// <summary>
		///// 
		///// </summary>
		//public Func<object, IAsyncResult, object> EndFunc { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Action<object, object> Action { get; set; }

		///// <summary>
		///// 
		///// </summary>
		//public Func<object, object, AsyncCallback, object, IAsyncResult> BeginAction { get; set; }
		////public Func<object, object, object, object, object> BeginAction { get; set; }

		///// <summary>
		///// 
		///// </summary>
		//public Action<object, IAsyncResult> EndAction { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Func<object> ServiceCreator { get; set; }

		///// <summary>
		///// 
		///// </summary>
		//public bool IsAsync { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsStatic { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsTask { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Type TaskResultType { get; set; }

		///// <summary>
		///// 
		///// </summary>
		//public Type ServiceType { get; set; }

		/// <summary>
		/// default value or argument type
		/// </summary>
		public object DefaultArgument { get; set; }

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
		public Task ExecuteAsync(ServiceContext context)
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

		private void ApplyExecutingFilters(ServiceContext context)
		{
			if (!(Service?.ActionExecteFilter?.Count > 0))
				return;

			foreach (var item in Service.ActionExecteFilter)
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
			if (!(Service?.ActionExecteFilter?.Count > 0))
				return;

			foreach (var item in Service.ActionExecteFilter)
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

		private object InvokeAction(ServiceContext context)
		{
			if (IsStatic)
			{
				return InvokeAction(context.Argument, null);
			}

#if NETCORE
			if (context.RequestServices != null)
			{
				var serviceInstance = context.RequestServices.GetService(MethodInfo.DeclaringType);
				if (serviceInstance == null)
				{
					throw new ServiceException("cannot resolve Service Instance from DI Container");
				}
				return InvokeAction(context.Argument, serviceInstance);
			}
#endif

			using (var serviceInstance = ServiceFactory.GetService(this))
			{
				return InvokeAction(context.Argument, serviceInstance.ServiceObject);
			}
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
			object serviceObject = null;
			if (!context.Action.IsStatic)
			{
#if NETCORE
				if (context.RequestServices != null)
				{
					serviceObject = context.RequestServices.GetService(MethodInfo.DeclaringType);
					if (serviceObject == null)
					{
						throw new ServiceException("cannot resolve Service Instance from DI Container");
					}
				}
				else
				{
#endif
					var serviceContainer = ServiceFactory.GetService(context.Action);
					context.ServiceContainer = serviceContainer;
					serviceObject = serviceContainer.ServiceObject;
#if NETCORE
				}
#endif
			}

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