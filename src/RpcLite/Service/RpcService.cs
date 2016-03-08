using System;
using System.IO;
using RpcLite.Formatters;
using RpcLite.Logging;

namespace RpcLite.Service
{
	/// <summary>
	/// Respresnts service infomation
	/// </summary>
	public class RpcService
	{
		#region properties

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

		#endregion

		/// <summary>
		/// Convert to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name}, {Path}, {Type}";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="cb"></param>
		/// <returns></returns>
		public IAsyncResult BeginProcessRequest(ServiceContext context, AsyncCallback cb)
		{
			LogHelper.Debug("RpcService.BeginProcessRequest");

			LogHelper.Debug("RpcService.BeginProcessRequest: start ActionHelper.GetActionInfo");
			var actionInfo = ActionHelper.GetActionInfo(context.Request.ServiceType, context.Request.ActionName);
			LogHelper.Debug("RpcService.BeginProcessRequest: end ActionHelper.GetActionInfo");
			if (actionInfo == null)
			{
				LogHelper.Debug("Action Not Found: " + context.Request.ActionName);
				throw new ActionNotFoundException(context.Request.ActionName);
			}

			object requestObject = null;
			if (actionInfo.ArgumentCount > 0)
				requestObject = GetRequestObject(context.Request.RequestStream, context.Formatter, actionInfo.ArgumentType);

			LogHelper.Debug("RpcService.BeginProcessRequest: got requestObject");

			context.Service = this;
			context.Action = actionInfo;
			context.Argument = requestObject;

			BeforeInvoke?.Invoke(context);

			var ar = actionInfo.Execute(context, cb);

			return ar;
		}


		//private static readonly QuickReadConcurrentDictionary<Type, Func<Task, object>> GetTaskResultFuncs 
		//	= new QuickReadConcurrentDictionary<Type, Func<Task, object>>();

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

		///// <summary>
		///// 
		///// </summary>
		///// <param name="serviceRequest"></param>
		///// <returns></returns>
		//public object ProcessRequest(ServiceRequest serviceRequest)
		//{
		//	var actionInfo = ActionHelper.GetActionInfo(serviceRequest.ServiceType, serviceRequest.ActionName);
		//	if (actionInfo == null)
		//		throw new ServiceException("Action Not Found: " + serviceRequest.ActionName);

		//	object requestObject = null;
		//	if (actionInfo.ArgumentCount > 0)
		//		requestObject = GetRequestObject(serviceRequest.RequestStream, serviceRequest.Formatter, actionInfo.ArgumentType);

		//	var result = ActionHelper.InvokeAction(actionInfo, requestObject);
		//	return actionInfo.HasReturnValue
		//		? result
		//		: NullResponse.Value;
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		public static void EndProcessRequest(IAsyncResult result)
		{
			var state = result.AsyncState as ServiceContext;
			if (state == null)
			{
				LogHelper.Error("ServiceContext is null", null);
				return;
			}

			if (state.Service == null)
				throw new InvalidOperationException("not implement, RpcLite bug");

			state.Service.EndProcessRequest(result, state);
		}

		private void EndProcessRequest(IAsyncResult ar, ServiceContext context)
		{
			//object resultObject = null;
			try
			{
				//resultObject = RpcAction.GetResultObject(ar, context);
				context.Result = RpcAction.GetResultObject(ar, context);
			}
			//catch (Exception)
			//{
			//	//var exObj = new ServiceExcepionResponse
			//	//{
			//	//	ErrorCode = 500,
			//	//	ExceptionAssembly = ex.GetType().Assembly.FullName,
			//	//	ExceptionType = ex.GetType().FullName,
			//	//	InnerException = ex,
			//	//	IsFrameworkExecption = ex is ServiceException,
			//	//};
			//	//context.Formatter.Serialize(context.Response.ResponseStream, exObj);
			//	//var httpContext = context.State as HttpContext;
			//	//if (httpContext != null)
			//	//{
			//	//	httpContext.Response.StatusCode = 500;
			//	//}
			//	throw;
			//}
			finally
			{
				AfterInvoke?.Invoke(context);

			}

		}
	}
}
