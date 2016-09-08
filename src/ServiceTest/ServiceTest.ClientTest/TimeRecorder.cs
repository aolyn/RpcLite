using System;
using System.Diagnostics;

namespace RpcLiteClientTestNetCore
{
	public class TimeRecorder : IDisposable
	{
		//private readonly DateTime _startDate;

		//public TimeRecorder()
		//{
		//	_startDate = DateTime.Now;
		//}

		//public void Dispose()
		//{
		//	var duration = DateTime.Now - _startDate;
		//	Console.WriteLine("Time escaped in ms " + duration.TotalMilliseconds.ToString("0.000"));
		//}
		private readonly Stopwatch _stopwatch;

		public TimeRecorder()
		{
			_stopwatch = Stopwatch.StartNew();
		}

		public void Dispose()
		{
			_stopwatch.Stop();
			Console.WriteLine("Time escaped in ms " + _stopwatch.Elapsed.TotalMilliseconds);
		}
	}
}
