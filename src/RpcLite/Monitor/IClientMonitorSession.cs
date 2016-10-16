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
		void OnInvoking(ClientContext request);

		/// <summary>
		/// 
		/// </summary>
		void OnInvoked(ClientContext request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		void OnSerializing(ClientContext request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		void OnSerialized(ClientContext request);
	}
}
