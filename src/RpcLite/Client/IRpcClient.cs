using RpcLite.Formatters;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IRpcClient
	{
		/// <summary>
		/// base url of service
		/// </summary>
		string Address { get; set; }

		/// <summary>
		/// Formatter
		/// </summary>
		IFormatter Formatter { get; set; }

		/// <summary>
		/// Channel to transport data with service
		/// </summary>
		IClientChannel Channel { get; set; }
	}
}