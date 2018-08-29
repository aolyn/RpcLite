namespace RpcLite.Filter
{
	/// <summary>
	/// 
	/// </summary>
	public interface IFilterFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IRpcFilter[] CreateFilters();
	}
}
