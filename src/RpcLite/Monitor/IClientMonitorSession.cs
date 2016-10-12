using RpcLite.Client;

namespace RpcLite.Monitor
{
	/// <summary>
	/// 
	/// </summary>
	public interface IClientMonitorSession
	{
		/// <summary>
		/// 
		/// </summary>
		void BeginInvoke(ClientContext request);

		/// <summary>
		/// 
		/// </summary>
		void EndInvoke(ClientContext request);
		void OnSerializing(ClientContext request);
		void OnSerialized(ClientContext request);
	}
}
