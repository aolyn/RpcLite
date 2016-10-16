using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RpcLite.Registry.Http
{
	internal class TaskHelper
	{
		private static readonly HashSet<Timer> Timers = new HashSet<Timer>();

		public static Task Delay(int timeout)
		{
			var tcs = new TaskCompletionSource<object>();
			Timer timer = null;
			timer = new Timer(o =>
			{
				tcs.SetResult(null);
				// ReSharper disable AccessToModifiedClosure
				Timers.Remove(timer);
				timer?.Dispose();
				// ReSharper restore AccessToModifiedClosure
			}, null, timeout, int.MaxValue);
			Timers.Add(timer);
			return tcs.Task;
		}

		public static Task<TResult> FromResult<TResult>(TResult result)
		{
			var tcs = new TaskCompletionSource<TResult>();
			tcs.SetResult(result);
			return tcs.Task;
		}

	}
}