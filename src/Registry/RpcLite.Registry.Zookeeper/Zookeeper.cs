using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using org.apache.zookeeper;
using org.apache.zookeeper.data;

namespace RpcLite.Registry.Zookeeper
{
	public class Zookeeper : IZookeeper
	{
		private readonly ZooKeeper _zookeeper;

		public Zookeeper(string connectstring, int sessionTimeout, Watcher watcher, bool canBeReadOnly = false)
		{
			_zookeeper = new ZooKeeper(connectstring, sessionTimeout, watcher, canBeReadOnly);
		}

		public Zookeeper(string connectString, int sessionTimeout, Watcher watcher, long sessionId, byte[] sessionPasswd, bool canBeReadOnly = false)
		{
			_zookeeper = new ZooKeeper(connectString, sessionTimeout, watcher, sessionId, sessionPasswd, canBeReadOnly);
		}

		public void AddAuthInfo(string scheme, byte[] auth)
		{
			_zookeeper.addAuthInfo(scheme, auth);
		}

		public Task CloseAsync()
		{
			return _zookeeper.closeAsync();
		}

		public Task<string> CreateAsync(string path, byte[] data, List<ACL> acl, CreateMode createMode)
		{
			return _zookeeper.createAsync(path, data, acl, createMode);
		}

		public Task DeleteAsync(string path, int version)
		{
			return _zookeeper.deleteAsync(path, version);
		}

		public Task<Stat> existsAsync(string path, bool watch)
		{
			throw new NotImplementedException();
		}

		public Task<Stat> ExistsAsync(string path, Watcher watcher)
		{
			return _zookeeper.existsAsync(path, watcher);
		}

		public Task<Stat> ExistsAsync(string path, bool watch = false)
		{
			return _zookeeper.existsAsync(path, watch);
		}

		public Task<ACLResult> GetAclAsync(string path)
		{
			return _zookeeper.getACLAsync(path);
		}

		public Task<ChildrenResult> GetChildrenAsync(string path, bool watch)
		{
			return _zookeeper.getChildrenAsync(path, watch);
		}

		public Task<ChildrenResult> GetChildrenAsync(string path, Watcher watcher)
		{
			return _zookeeper.getChildrenAsync(path, watcher);
		}

		public List<string> GetChildWatches()
		{
			throw new NotImplementedException();
		}

		public Task<DataResult> GetDataAsync(string path, bool watch)
		{
			return _zookeeper.getDataAsync(path, watch);
		}

		public Task<DataResult> GetDataAsync(string path, Watcher watcher)
		{
			throw new NotImplementedException();
		}

		public List<string> GetDataWatches()
		{
			throw new NotImplementedException();
		}

		public List<string> GetExistWatches()
		{
			throw new NotImplementedException();
		}

		public long GetSessionId()
		{
			throw new NotImplementedException();
		}

		public byte[] GetSessionPasswd()
		{
			throw new NotImplementedException();
		}

		public int GetSessionTimeout()
		{
			throw new NotImplementedException();
		}

		public ZooKeeper.States GetState()
		{
			throw new NotImplementedException();
		}

		public Task<List<OpResult>> MultiAsync(List<Op> ops)
		{
			throw new NotImplementedException();
		}

		public string PrependChroot(string clientPath)
		{
			throw new NotImplementedException();
		}

		public Task<Stat> SetAclAsync(string path, List<ACL> acl, int version)
		{
			throw new NotImplementedException();
		}

		public Task<Stat> SetDataAsync(string path, byte[] data, int version)
		{
			throw new NotImplementedException();
		}

		public Task Sync(string path)
		{
			throw new NotImplementedException();
		}

		public Transaction Transaction()
		{
			throw new NotImplementedException();
		}

		public List<OpResult> ValidatePath(List<Op> ops)
		{
			throw new NotImplementedException();
		}

		public Op WithRootPrefix(Op op)
		{
			throw new NotImplementedException();
		}
	}
}

namespace System
{
	public static class StringExtensionFunc
	{
		public static byte[] GetBytes(this string source)
		{
			return source == null
				? null
				: Encoding.UTF8.GetBytes(source);
		}
	}
}
