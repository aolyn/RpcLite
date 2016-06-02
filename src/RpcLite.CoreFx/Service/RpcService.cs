using System;
using System.Threading.Tasks;
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
			var action = ActionManager.GetAction(context.Request.ServiceType, context.Request.ActionName);
			LogHelper.Debug("RpcService.BeginProcessRequest: end ActionHelper.GetActionInfo");
			if (action == null)
			{
				if (context.Request.ActionName == "?meta")
				{

				}

				LogHelper.Debug("Action Not Found: " + context.Request.ActionName);
				throw new ActionNotFoundException(context.Request.ActionName);
			}

			LogHelper.Debug("RpcService.BeginProcessRequest: got requestObject");

			context.Service = this;
			context.Action = action;

			try
			{
				BeforeInvoke?.Invoke(context);
			}
			catch (Exception ex)
			{
				LogHelper.Error(ex);
			}

			var task = action.ExecuteAsync(context);
			var waitTask = task.ContinueWith(tsk =>
			{
				try
				{
					AfterInvoke?.Invoke(context);
				}
				catch (Exception ex)
				{
					LogHelper.Error(ex);
				}

				//if (tsk.IsFaulted)
				//{
				//	context.Exception = tsk.Exception.InnerException;
				//}
				//else
				//{
				//	var result = RpcAction.GetResultObject(tsk, context);
				//	context.Result = result;
				//}
			});

			return waitTask;
		}

	}
}
