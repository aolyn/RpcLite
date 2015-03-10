using System;
using System.IO;
using System.Threading.Tasks;
using RpcLite.Formatters;

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
			//Hrj.Logging.Logger.Debug("RpcService.BeginProcessRequest");

			//Hrj.Logging.Logger.Debug("RpcService.BeginProcessRequest: start ActionHelper.GetActionInfo");
			var actionInfo = ActionHelper.GetActionInfo(request.ServiceType, request.ActionName);
			//Hrj.Logging.Logger.Debug("RpcService.BeginProcessRequest: end ActionHelper.GetActionInfo");
			if (actionInfo == null)
			{
				//Hrj.Logging.Logger.Debug("Action Not Found: " + request.ActionName);
				throw new ServiceException("Action Not Found: " + request.ActionName);
			}

			object requestObject = null;
			if (actionInfo.ArgumentCount > 0)
				requestObject = GetRequestObject(request.InputStream, request.Formatter, actionInfo.ArgumentType);

			//Hrj.Logging.Logger.Debug("RpcService.BeginProcessRequest: got requestObject");

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

			IAsyncResult ar = actionInfo.ExecuteTask(context, cb);

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

		private void EndProcessRequest(IAsyncResult ar, ServiceContext context)
		{
			object resultObject = ActionInfo.GetResultObject(ar, context);

			if (AfterInvoke != null)
				AfterInvoke(context);

			if (context.Action.HasReturnValue)
			{
				context.Response.Formatter.Serialize(context.Response.ResponseStream, resultObject);
			}
		}
	}
}
