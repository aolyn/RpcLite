using System.Threading.Tasks;

namespace RpcLite.Registry.Zookeeper
{
	internal class TimeSignalDelay
	{
		private TaskCompletionSource<object> _tcs;
		private bool _signal;
		private Task _waitTask;

		public Task Delay(int timeout)
		{
			if (_signal)
			{
				_signal = false;
#if NETCORE
				return Task.CompletedTask;
#else
				return Task.FromResult<object>(null);
#endif
			}

			var task1 = Task.Delay(timeout);
			_tcs = new TaskCompletionSource<object>();
			_waitTask = Task.WhenAny(task1, _tcs.Task);
			return _waitTask;
		}

		public void Set()
		{
			if (_waitTask != null && !_waitTask.IsCompleted)
				_tcs?.SetResult(null);
			else
				_signal = true;
		}
	}
}
