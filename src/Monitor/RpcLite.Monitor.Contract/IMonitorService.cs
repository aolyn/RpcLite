using System.Threading.Tasks;

namespace RpcLite.Monitor.Contract
{
	public interface IMonitorService
	{
		Task AddInvokesAsync(InvokeInfo[] invokes);
	}
}
