using RpcLite.Config;

namespace RpcLite.Service
{
	/// <summary>
	/// current request path info
	/// </summary>
	public class RequestPathInfo
	{
		/// <summary>
		/// service of current request
		/// </summary>
		public string Service { get; set; }

		/// <summary>
		/// action name
		/// </summary>
		public string Action { get; set; }

		/// <summary>
		/// query
		/// </summary>
		public string Query { get; set; }

		/// <summary>
		/// path info
		/// </summary>
		public string PathInfo { get; set; }
	}
}
