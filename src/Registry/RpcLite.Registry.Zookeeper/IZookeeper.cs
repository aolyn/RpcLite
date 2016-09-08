using System.Collections.Generic;
using System.Threading.Tasks;
using org.apache.zookeeper;
using org.apache.zookeeper.data;

namespace RpcLite.Registry.Zookeeper
{
	public interface IZookeeper
	{
		Task<Stat> ExistsAsync(string path, bool watch = false);
		List<string> GetDataWatches();
		List<string> GetExistWatches();
		List<string> GetChildWatches();
		long GetSessionId();
		byte[] GetSessionPasswd();
		int GetSessionTimeout();
		void AddAuthInfo(string scheme, byte[] auth);
		Task CloseAsync();
		string PrependChroot(string clientPath);
		Task<string> CreateAsync(string path, byte[] data, List<ACL> acl, CreateMode createMode);
		Task DeleteAsync(string path, int version);
		Task<List<OpResult>> MultiAsync(List<Op> ops);
		List<OpResult> ValidatePath(List<Op> ops);
		//MultiTransactionRecord generateMultiTransaction(List<Op> ops);
		Op WithRootPrefix(Op op);
		//Task<List<OpResult>> multiInternal(MultiTransactionRecord request);
		Transaction Transaction();
		Task<Stat> ExistsAsync(string path, Watcher watcher);
		//Task<Stat> existsAsync(string path, bool watch);
		Task<DataResult> GetDataAsync(string path, Watcher watcher);
		Task<DataResult> GetDataAsync(string path, bool watch);
		Task<Stat> SetDataAsync(string path, byte[] data, int version);
		Task<ACLResult> GetAclAsync(string path);
		Task<Stat> SetAclAsync(string path, List<ACL> acl, int version);
		Task<ChildrenResult> GetChildrenAsync(string path, Watcher watcher);
		Task<ChildrenResult> GetChildrenAsync(string path, bool watch);
		Task Sync(string path);
		ZooKeeper.States GetState();
		string ToString();
	}
}