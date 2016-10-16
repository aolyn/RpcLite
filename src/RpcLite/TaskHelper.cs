using System.Threading.Tasks;

#if NET40
using System.Collections.Generic;
using System.Threading;
#endif

namespace RpcLite
{
	/// <summary>
	/// 
	/// </summary>
	public static class TaskHelper
	{
#if NET40
		private static readonly HashSet<Timer> Timers = new HashSet<Timer>();
#endif

		/// <summary>
		/// get task that returns a assigned result
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="result"></param>
		/// <returns></returns>
		public static Task<TResult> FromResult<TResult>(TResult result)
		{
#if NETCORE
			return Task.FromResult(result);
#else
			var tcs = new TaskCompletionSource<TResult>();
			tcs.SetResult(result);
			return tcs.Task;
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="millisecondsDelay"></param>
		/// <returns></returns>
		public static Task Delay(int millisecondsDelay)
		{
#if NET40
			var tcs = new TaskCompletionSource<object>();
			Timer timer = null;
			timer = new Timer(o =>
			{
				tcs.SetResult(null);
				// ReSharper disable AccessToModifiedClosure
				Timers.Remove(timer);
				timer?.Dispose();
				// ReSharper restore AccessToModifiedClosure
			}, null, millisecondsDelay, int.MaxValue);
			Timers.Add(timer);
			return tcs.Task;
#else
			return Task.Delay(millisecondsDelay);
#endif
		}

	}
}
