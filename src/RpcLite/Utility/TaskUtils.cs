// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks
{
	internal static class TaskUtils
	{
		public static Task<TOut> ContinueWithTask<TIn, TOut>(this Task<TIn> src, Func<Task<TIn>, Task<TOut>> continuation)
		{
			var tcs = new TaskCompletionSource<TOut>();
			src.ContinueWith(tsk =>
			{
				try
				{
					var nextTask = continuation(tsk);
					nextTask.ContinueWith(tsk2 =>
					{
						try
						{
							tcs.SetResult(tsk2.Result);
						}
						catch (Exception ex)
						{
							tcs.SetException(ex);
						}
					});
				}
				catch (Exception ex)
				{
					tcs.SetException(ex);
				}
			});
			return tcs.Task;
		}
	}
}
