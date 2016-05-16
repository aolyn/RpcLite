
namespace RpcLite
{
	/// <summary>
	/// Represents a Response with no data retrieve
	/// </summary>
	public class NullResponse
	{
		/// <summary>
		/// Represents the sole instance of RpcLite.NullResponse
		/// </summary>
		public static readonly NullResponse Value = new NullResponse();
		private NullResponse() { }
	}
}
