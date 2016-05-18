using System;
using System.IO;
using System.Threading.Tasks;
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

		/// <summary>
		/// Convert to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name}, {Path}, {Type}";
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public Task ProcessRequestAsync(ServiceContext context)
		{
			LogHelper.Debug("RpcService.BeginProcessRequest");

			LogHelper.Debug("RpcService.BeginProcessRequest: start ActionHelper.GetActionInfo");
			var actionInfo = ActionManager.GetAction(context.Request.ServiceType, context.Request.ActionName);
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

			try
			{
				BeforeInvoke?.Invoke(context);
			}
			catch (Exception ex)
			{
				LogHelper.Error(ex);
			}

			var ar = actionInfo.ExecuteAsync(context);
			var waitTask = ar.ContinueWith(tsk =>
			{
				try
				{
					AfterInvoke?.Invoke(context);
				}
				catch (Exception ex)
				{
					LogHelper.Error(ex);
				}

				if (tsk.IsFaulted)
				{
					context.Exception = tsk.Exception.InnerException;
				}
				else
				{
					var result = RpcAction.GetResultObject(tsk, context);
					context.Result = result;
				}
			});

			return waitTask;
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

		///// <summary>
		///// 
		///// </summary>
		///// <param name="result"></param>
		//public static void EndProcessRequest(IAsyncResult result)
		//{
		//	var state = result.AsyncState as ServiceContext;
		//	if (state == null)
		//	{
		//		LogHelper.Error("ServiceContext is null", null);
		//		return;
		//	}

		//	if (state.Service == null)
		//		throw new InvalidOperationException("not implement, RpcLite bug");

		//	state.Service.EndProcessRequest(result, state);
		//}

		//private void EndProcessRequest(IAsyncResult ar, ServiceContext context)
		//{
		//	//object resultObject = null;
		//	try
		//	{
		//		//resultObject = RpcAction.GetResultObject(ar, context);
		//		//context.Result = RpcAction.GetResultObject(ar, context);
		//	}
		//	finally
		//	{
		//		AfterInvoke?.Invoke(context);
		//	}
		//}

	}
}
