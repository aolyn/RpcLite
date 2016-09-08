using System;

namespace RpcLite.Registry.Zookeeper
{
	public class ZookeeperException : Exception
	{
		public ZookeeperException(string message)
			: base(message) { }

		public ZookeeperException(string message, Exception inner)
			: base(message, inner) { }
	}

	public class ConnectionLossException : ZookeeperException
	{
		public ConnectionLossException(string message)
			: base(message) { }

		public ConnectionLossException(string message, Exception inner)
			: base(message, inner) { }
	}

	public class SessionExpireException : ZookeeperException
	{
		public SessionExpireException(string message)
			: base(message) { }

		public SessionExpireException(string message, Exception inner)
			: base(message, inner) { }
	}
}
