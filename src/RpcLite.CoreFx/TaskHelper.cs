using System.Threading.Tasks;

namespace RpcLite
{
	/// <summary>
	/// 
	/// </summary>
	public static class TaskHelper
	{
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
	}
}
