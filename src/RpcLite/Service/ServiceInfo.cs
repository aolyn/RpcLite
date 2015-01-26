using System;
using System.IO;
using RpcLite.Formatters;

namespace RpcLite.Service
{
	/// <summary>
	/// Respresnts service infomation
	/// </summary>
	public class ServiceInfo
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
			var actionInfo = ActionHelper.GetActionInfo(request.ServiceType, request.ActionName);
			if (actionInfo == null)
				throw new ServiceException("Action Not Found: " + request.ActionName);

			object requestObject = null;
			if (actionInfo.ArgumentCount > 0)
				requestObject = GetRequestObject(request.InputStream, request.Formatter, actionInfo.ArgumentType);

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
			if (actionInfo.IsAsync)
			{
				ar = ActionHelper.BeginInvokeAction(actionInfo, response, requestObject, cb, context);
			}
			else
			{
				context.Result = ActionHelper.InvokeAction(actionInfo, requestObject);
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
			if (!state.Action.IsAsync)
			{
				if (AfterInvoke != null)
					AfterInvoke(state);

				if (state.Action.HasReturnValue)
				{
					state.Response.Formatter.Serialize(state.Response.ResponseStream, state.Result);
				}
				return;
			}

			var serviceContainer = (ServiceInstanceContainer)state.ServiceContainer;
			if (serviceContainer == null) return;

			try
			{
				if (state.Action.HasReturnValue)
				{
					object requestResult;

					try
					{
						requestResult = state.Action.EndFunc(serviceContainer.ServiceObject, result);
					}
					catch (Exception ex)
					{
						requestResult = ex;
					}

					state.Result = requestResult;

					if (AfterInvoke != null)
						AfterInvoke(state);
					state.Response.Formatter.Serialize(state.Response.ResponseStream, requestResult);
				}
				else
				{
					state.Action.EndAction(serviceContainer.ServiceObject, result);
					if (AfterInvoke != null)
						AfterInvoke(state);
				}
			}
			catch (Exception ex)
			{
				state.Response.Formatter.Serialize(state.Response.ResponseStream, ex);
			}
			serviceContainer.Dispose();
		}
	}
}
