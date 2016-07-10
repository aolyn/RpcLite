using System;

namespace RpcLiteClientTestNetCore
{
	public class TimeRecorder : IDisposable
	{
		private readonly DateTime _startDate;

		public TimeRecorder()
		{
			_startDate = DateTime.Now;
		}

		public void Dispose()
		{
			var duration = DateTime.Now - _startDate;
			Console.WriteLine("Time escaped in ms " + duration.TotalMilliseconds);
		}
	}
}
