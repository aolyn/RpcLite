using System;
using System.IO;
using RpcLite.Formatters;
using System.Threading.Tasks;

namespace RpcLite.Service
{
	/// <summary>
	/// Respresnts service infomation
	/// </summary>
	public class RpcService
	{
		/// <summary>
		/// Service request url path
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Service's Type
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// Name of Service
		/// </summary>
		public string Name { get; set; }

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
			return string.Format("{0}, {1}, {2}", Name, Path, Type);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <param name="cb"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public IAsyncResult BeginProcessRequest(ServiceRequest request, ServiceResponse response, AsyncCallback cb, object state)
		{
			Hrj.Logging.Logger.Debug("RpcService.BeginProcessRequest");

			Hrj.Logging.Logger.Debug("RpcService.BeginProcessRequest: start ActionHelper.GetActionInfo");
			var actionInfo = ActionHelper.GetActionInfo(request.ServiceType, request.ActionName);
			Hrj.Logging.Logger.Debug("RpcService.BeginProcessRequest: end ActionHelper.GetActionInfo");
			if (actionInfo == null)
			{
				Hrj.Logging.Logger.Debug("Action Not Found: " + request.ActionName);
				throw new ServiceException("Action Not Found: " + request.ActionName);
			}

			object requestObject = null;
			if (actionInfo.ArgumentCount > 0)
				requestObject = GetRequestObject(request.InputStream, request.Formatter, actionInfo.ArgumentType);

			Hrj.Logging.Logger.Debug("RpcService.BeginProcessRequest: got requestObject");

			var context = new ServiceContext
			{
				Service = this,
				Request = request,
				Response = response,
				Action = actionInfo,
				Argument = requestObject,
				State = state,
			};

			if (BeforeInvoke != null)
				BeforeInvoke(context);

			IAsyncResult ar;
			if (actionInfo.IsTask)
			{
				var task = (Task)ActionHelper.InvokeTask(actionInfo, response, requestObject, cb, context);
				ar = ToBegin(task, cb, context/*, actionInfo.TaskResultType*/);
			}
			else if (actionInfo.IsAsync)
			{
				ar = ActionHelper.BeginInvokeAction(actionInfo, response, requestObject, cb, context);
			}
			else
			{
				Hrj.Logging.Logger.Debug("RpcService.BeginProcessRequest: start ActionHelper.InvokeAction");
				context.Result = ActionHelper.InvokeAction(actionInfo, requestObject);
				Hrj.Logging.Logger.Debug("RpcService.BeginProcessRequest: end ActionHelper.InvokeAction");
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
		private static object GetTaskResult(Task task)
		{
			if (task.GetType() == typeof(Task))
			{
				return null;
			}
			else
			{
				return ((dynamic)task).Result;
			}
		}

		private static object GetRequestObject(Stream stream, IFormatter formatter, Type type)
		{
			try
			{
				return formatter.Deserilize(stream, type);
			}
			catch (Exception)
			{
				throw new RequestException("Parse request content to object error.");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceRequest"></param>
		/// <returns></returns>
		public object ProcessRequest(ServiceRequest serviceRequest)
		{
			var actionInfo = ActionHelper.GetActionInfo(serviceRequest.ServiceType, serviceRequest.ActionName);
			if (actionInfo == null)
				throw new ServiceException("Action Not Found: " + serviceRequest.ActionName);

			object requestObject = null;
			if (actionInfo.ArgumentCount > 0)
				requestObject = GetRequestObject(serviceRequest.InputStream, serviceRequest.Formatter, actionInfo.ArgumentType);

			var result = ActionHelper.InvokeAction(actionInfo, requestObject);
			return actionInfo.HasReturnValue
				? result
				: NullResponse.Value;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		public static void EndProcessRequest(IAsyncResult result)
		{
			var state = result.AsyncState as ServiceContext;
			if (state == null) return;

			if (state.Service == null)
				throw new InvalidOperationException("not implement, RpcLite bug");

			state.Service.EndProcessRequest(result, state);
		}

		private void EndProcessRequest(IAsyncResult result, ServiceContext state)
		{
			var serviceContainer = (ServiceInstanceContainer)state.ServiceContainer;
			object resultObject = null;

			try
			{
				if (state.Action.IsTask)
				{
					var task = result as Task;
					if (task == null)
						throw new ServiceException("task async api return no Task result");

					resultObject = GetTaskResult(task);
				}
				else if (!state.Action.IsAsync)
				{
					if (state.Action.HasReturnValue)
					{
						resultObject = state.Result;
					}
				}
				else
				{
					if (state.Action.HasReturnValue)
					{
						try
						{
							resultObject = state.Action.EndFunc(serviceContainer.ServiceObject, result);
						}
						catch (Exception ex)
						{
							resultObject = ex;
						}
					}
					else
					{
						state.Action.EndAction(serviceContainer.ServiceObject, result);
					}
				}
			}
			catch (Exception ex)
			{
				resultObject = ex;
			}

			if (state.Action.IsAsync && !state.Action.IsStatic)
			{
				serviceContainer.Dispose();
			}

			if (AfterInvoke != null)
				AfterInvoke(state);

			if (state.Action.HasReturnValue)
			{
				state.Response.Formatter.Serialize(state.Response.ResponseStream, resultObject);
			}
		}
	}
}
