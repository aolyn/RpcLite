using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
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

		/// <summary>
		/// 
		/// </summary>
		public Type ServiceType { get; set; }

		#endregion

		internal Task ExecuteAsync(ServiceContext context)
		{
			if (IsTask)
			{
				var task = ActionHelper.InvokeTask(context);
				return task;
			}
			else
			{
				LogHelper.Debug("RpcService.BeginProcessRequest: start ActionHelper.InvokeAction");
				try
				{
					context.Result = ActionHelper.InvokeAction(context.Action, context.Argument);
				}
				catch (Exception ex)
				{
					var tcs = new TaskCompletionSource<object>();
					tcs.SetException(ex);
					return tcs.Task;
				}
				LogHelper.Debug("RpcService.BeginProcessRequest: end ActionHelper.InvokeAction");

				var task = new Task<object>(() => context.Result);
				task.RunSynchronously();
				return task;
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
			//return task.GetType() == typeof(Task)
			//	? null
			//	: ((dynamic)task).Result;

			try
			{
				var type = task.GetType();
				var func = GetTaskResultFuncs.GetOrAdd(type, () =>
				{
					//var taskType = typeof(Task<>).MakeGenericType(type);
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
			//return null;
		}

		internal static object GetResultObject(IAsyncResult ar, ServiceContext context)
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
				else // if (!context.Action.IsAsync)
				{
					if (context.Action.HasReturnValue)
					{
						resultObject = context.Result;
					}
				}
				//else
				//{
				//	if (context.Action.HasReturnValue)
				//	{
				//		try
				//		{
				//			resultObject = context.Action.EndFunc(serviceContainer.ServiceObject, ar);
				//		}
				//		catch (Exception ex)
				//		{
				//			resultObject = ex;
				//		}
				//	}
				//	else
				//	{
				//		context.Action.EndAction(serviceContainer.ServiceObject, ar);
				//	}
				//}
			}
			//catch (Exception ex)
			//{
			//	resultObject = ex;
			//}
			finally
			{
				if (/*context.Action.IsAsync &&*/ serviceContainer != null && !context.Action.IsStatic)
				{
					serviceContainer.Dispose();
				}
			}

			return resultObject;
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