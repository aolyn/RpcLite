// ReSharper disable once CheckNamespace
namespace System.Diagnostics
{
	public static class StopwatchExtensionFunc
	{
		public static TimeSpan GetAndRest(this Stopwatch stopwatch)
		{
			var ts = stopwatch.Elapsed;
			stopwatch.Reset();
			stopwatch.Restart();
			return ts;
		}
	}
}
