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
				context.Argument = context.Request.ContentLength > 0
					? GetRequestObject(context.Request.RequestStream, context.Formatter, ArgumentType)
					: DefaultArgument;
			}

			if (IsTask)
			{
				try
				{
					var task = InvokeTaskInternal(context);

					var waitTask = task.ContinueWith(tsk =>
					{
						if (tsk.IsFaulted)
						{
							context.Exception = tsk.Exception.InnerException;
						}
						else
						{
							var result = GetResultObject(tsk, context);
							context.Result = result;
						}
					});

					return waitTask;
				}
				catch (Exception ex)
				{
					context.Exception = ex;

					var tcs = new TaskCompletionSource<object>();
					tcs.SetResult(null);
					return tcs.Task;
				}
			}
			else
			{
				LogHelper.Debug("RpcService.BeginProcessRequest: start ActionHelper.InvokeAction");
				try
				{
					context.Result = InvokeAction(context.Argument);
				}
				catch (Exception ex)
				{
					context.Exception = ex;

					var tcs = new TaskCompletionSource<object>();
					tcs.SetResult(null);
					return tcs.Task;
				}
				LogHelper.Debug("RpcService.BeginProcessRequest: end ActionHelper.InvokeAction");

				//var task = new Task<object>(() => context.Result);
				//task.RunSynchronously();
				//return task;

				return TaskHelper.FromResult(context.Result);
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
		internal static object GetTaskResult(Task task)
		{
			try
			{
				var type = task.GetType();
				var func = GetTaskResultFuncs.GetOrAdd(type, () =>
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
				if (aex != null)
					throw ex.InnerException;
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

		private object InvokeAction(object reqArg)
		{
			if (IsStatic)
			{
				return InvokeAcion(reqArg, null);
			}

			using (var serviceInstance = ServiceFactory.GetService(this))
			{
				return InvokeAcion(reqArg, serviceInstance.ServiceObject);
			}
		}

		private object InvokeAcion(object requestObject, object serviceObject)
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

		private static Task InvokeTaskInternal(ServiceContext context)
		{
			object serviceObject = null;
			if (!context.Action.IsStatic)
			{
				var serviceContainer = ServiceFactory.GetService(context.Action);
				context.ServiceContainer = serviceContainer;
				serviceObject = serviceContainer.ServiceObject;
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