using RpcLite.Formatters;
using RpcLite.Monitor;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public class ClientContext
	{
		/// <summary>
		/// 
		/// </summary>
		public object Request;

		/// <summary>
		/// 
		/// </summary>
		public RpcActionInfo Action;

		/// <summary>
		/// 
		/// </summary>
		public IFormatter Formatter;

		/// <summary>
		/// 
		/// </summary>
		public IClientMonitorSession Monitor;
	}
}
