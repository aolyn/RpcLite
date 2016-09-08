// ReSharper disable once CheckNamespace
namespace System.Diagnostics
{
	/// <summary>
	/// 
	/// </summary>
	public static class StopwatchExtensionFunc
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stopwatch"></param>
		/// <returns></returns>
		public static TimeSpan GetAndRest(this Stopwatch stopwatch)
		{
			var ts = stopwatch.Elapsed;
			stopwatch.Reset();
			stopwatch.Restart();
			return ts;
		}
	}
}
