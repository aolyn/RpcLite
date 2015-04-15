using System;
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

		/// <summary>
		/// T: service, argument, callback, state, return
		/// </summary>
		public Func<object, object, AsyncCallback, object, IAsyncResult> BeginFunc { get; set; }

		//public Func<object, object, object, object, object> BeginFunc { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Func<object, IAsyncResult, object> EndFunc { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Action<object, object> Action { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Func<object, object, AsyncCallback, object, IAsyncResult> BeginAction { get; set; }
		//public Func<object, object, object, object, object> BeginAction { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Action<object, IAsyncResult> EndAction { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Func<object> ServiceCreator { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsAsync { get; set; }

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

		#endregion

		internal Task ExecuteTask(ServiceResponse response, object requestObject, AsyncCallback cb, ServiceContext context)
		{
			var task = (Task)ActionHelper.InvokeTask(context, cb);
			return task;
		}

		internal IAsyncResult Execute(ServiceContext context, AsyncCallback callback)
		{
			IAsyncResult ar;
			if (IsTask)
			{
				//var task = (Task)ActionHelper.InvokeTask(actionInfo, response, requestObject, cb, context);
				var task = ExecuteTask(context.Response, context.Argument, callback, context);
				ar = ToBegin(task, callback, context/*, actionInfo.TaskResultType*/);
			}
			else if (IsAsync)
			{
				ar = ActionHelper.BeginInvokeAction(context.Action, context.Response, context.Argument, callback, context);
			}
			else
			{
				LogHelper.Debug("RpcService.BeginProcessRequest: start ActionHelper.InvokeAction");
				context.Result = ActionHelper.InvokeAction(context.Action, context.Argument);
				LogHelper.Debug("RpcService.BeginProcessRequest: end ActionHelper.InvokeAction");
				ar = new ServiceAsyncResult
				{
					AsyncState = context,
					IsCompleted = true,
					CompletedSynchronously = true,
					AsyncWaitHandle = null,
				};
			}
			return ar;
		}

		private static IAsyncResult ToBegin(Task task, AsyncCallback callback, object state)
		{
			if (task == null)
				throw new ArgumentNullException("task");

			var tcs = new TaskCompletionSource<object>(state);
			task.ContinueWith(t =>
			{
				if (task.IsFaulted)
				{
					if (task.Exception != null)
						tcs.TrySetException(task.Exception.InnerExceptions);
				}
				else if (task.IsCanceled)
					tcs.TrySetCanceled();
				else
					tcs.TrySetResult(GetTaskResult(t));

				if (callback != null)
					callback(tcs.Task);
			}, TaskScheduler.Default);

			return tcs.Task;
		}

		/// <summary>
		/// get result of task
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		internal static object GetTaskResult(Task task)
		{
			return task.GetType() == typeof(Task)
				? null
				: ((dynamic)task).Result;

			//try
			//{
			//	var type = task.GetType();
			//	GetTaskResultFuncs.GetOrAdd(type, () =>
			//	{
			//		var arg = Expression.Parameter(type, "task");
			//		var expProp = Expression.Property(arg, "Result");
			//		var expConvert = Expression.Convert(expProp, typeof(object));
			//		var asignExp = Expression.Lambda(expConvert, arg);
			//		var getFunc = asignExp.Compile();
			//		return (Func<Task, object>)getFunc;
			//	});
			//	//var value = getFunc(task);
			//}
			//catch (Exception ex)
			//{
			//}
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
				else if (!context.Action.IsAsync)
				{
					if (context.Action.HasReturnValue)
					{
						resultObject = context.Result;
					}
				}
				else
				{
					if (context.Action.HasReturnValue)
					{
						try
						{
							resultObject = context.Action.EndFunc(serviceContainer.ServiceObject, ar);
						}
						catch (Exception ex)
						{
							resultObject = ex;
						}
					}
					else
					{
						context.Action.EndAction(serviceContainer.ServiceObject, ar);
					}
				}
			}
			catch (Exception ex)
			{
				resultObject = ex;
			}

			if (context.Action.IsAsync && !context.Action.IsStatic)
			{
				serviceContainer.Dispose();
			}

			return resultObject;
		}

	}
}