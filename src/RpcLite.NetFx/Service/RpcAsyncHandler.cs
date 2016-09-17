#if !NETCORE

using System;
using System.Threading.Tasks;
using System.Web;
using RpcLite.AspNet;

namespace RpcLite.Service
{
	/// <summary>
	/// <para>RpcLite AsyncHandler to process service request</para>
	/// <para>in mono 3.12 RpcAsyncHandler can't work use Sync Handler</para>
	/// </summary>
	public class RpcAsyncHandler : IHttpAsyncHandler
	{
		static RpcAsyncHandler()
		{
			RpcInitializer.Initialize();
		}

		#region Sync

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public void ProcessRequest(HttpContext context)
		{
			RpcManager.ProcessAsync(new AspNetServerContext(context)).Wait();
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsReusable { get { return true; } }

		#endregion

		private static IAsyncResult ProcessReqeustInternal(HttpContext context, AsyncCallback cb, object extraData)
		{
			var task = RpcManager.ProcessAsync(new AspNetServerContext(context));
			var waitTask = ToBegin(task, cb, extraData);
			return waitTask;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="task"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		/// <returns></returns>
		public static IAsyncResult ToBegin(Task task, AsyncCallback callback, object state)
		{
			if (task == null)
				throw new ArgumentNullException(nameof(task));

			var tcs = new TaskCompletionSource<object>(state);
			task.ContinueWith(t =>
			{
				if (task.IsFaulted)
				{
					if (task.Exception != null)
						tcs.TrySetException(task.Exception.InnerExceptions);
				}
				else if (task.IsCanceled)
				{
					tcs.TrySetCanceled();
				}
				else
				{
					tcs.TrySetResult(null);
					//tcs.TrySetResult(RpcAction.GetTaskResult(t));
				}

				callback?.Invoke(tcs.Task);
			}/*, TaskScheduler.Default*/);

			return tcs.Task;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="cb"></param>
		/// <param name="extraData"></param>
		/// <returns></returns>
		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			return ProcessReqeustInternal(context, cb, extraData);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		public void EndProcessRequest(IAsyncResult result)
		{
		}

	}
}

#endif
