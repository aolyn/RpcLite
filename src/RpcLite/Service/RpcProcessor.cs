using System;
using System.IO;
using System.Linq;
using RpcLite.Config;
using RpcLite.Formatters;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	internal static class RpcProcessor
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceRequest"></param>
		/// <returns></returns>
		public static object ProcessRequest(ServiceRequest serviceRequest)
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
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <param name="cb"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public static IAsyncResult BeginProcessRequest(ServiceRequest request, ServiceResponse response, AsyncCallback cb, object state)
		{
			var actionInfo = ActionHelper.GetActionInfo(request.ServiceType, request.ActionName);
			if (actionInfo == null)
				throw new ServiceException("Action Not Found: " + request.ActionName);

			object requestObject = null;
			if (actionInfo.ArgumentCount > 0)
				requestObject = GetRequestObject(request.InputStream, request.Formatter, actionInfo.ArgumentType);

			if (actionInfo.IsAsync)
			{
				var result = ActionHelper.BeginInvokeAction(actionInfo, response, requestObject, cb, state);
				return result;
			}
			else
			{
				var result = ActionHelper.InvokeAction(actionInfo, requestObject);
				return new ServiceAsyncResult
				{
					AsyncState = new SeviceInvokeContext
					{
						SyncResult = result,
						Action = actionInfo,
						Response = response,
					},
					IsCompleted = true,
					CompletedSynchronously = true,
					AsyncWaitHandle = null,
				};
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
		/// <param name="result"></param>
		public static void EndProcessRequest(IAsyncResult result)
		{
			var state = result.AsyncState as SeviceInvokeContext;

			if (state == null) return;

			if (!state.Action.IsAsync)
			{
				if (state.Action.HasReturnValue)
				{
					state.Response.Formatter.Serialize(state.Response.ResponseStream, state.SyncResult);
				}
				return;
			}

			var service = (ServiceInstanceContainer)state.Service;
			if (service == null) return;

			try
			{
				if (state.Action.HasReturnValue)
				{
					object requestResult;

					try
					{
						requestResult = state.Action.EndFunc(service.ServiceObject, result);
					}
					catch (Exception ex)
					{
						requestResult = ex;
					}
					state.Response.Formatter.Serialize(state.Response.ResponseStream, requestResult);
				}
				else
				{
					state.Action.EndAction(service.ServiceObject, result);
				}
			}
			catch (Exception ex)
			{
				state.Response.Formatter.Serialize(state.Response.ResponseStream, ex);
			}
			service.Dispose();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		public static IFormatter GetFormatter(string contentType)
		{
			IFormatter formatter;
			if (!string.IsNullOrEmpty(contentType))
			{
				formatter = GlobalConfig.Formaters.FirstOrDefault(it => it.SupportMimes.Contains(contentType));
				if (formatter == null)
					throw new ConfigException("Not Supported MIME: " + contentType);
			}
			else
			{
				if (GlobalConfig.Formaters.Count == 0)
					throw new ConfigException("Configuration error: no formatters.");

				formatter = GlobalConfig.Formaters[0];
				if (formatter.SupportMimes.Count == 0)
					throw new ConfigException("Configuration error: formatter " + formatter.GetType() + " has no support MIME");
			}
			return formatter;
		}
	}
}