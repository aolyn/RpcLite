namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public interface IServiceMapper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceContext"></param>
		/// <returns></returns>
		MapServiceResult MapService(ServiceContext serviceContext);
	}

	/// <summary>
	/// 
	/// </summary>
	public struct MapServiceResult
	{
		/// <summary>
		/// 
		/// </summary>
		public static MapServiceResult Empty;

		/// <summary>
		/// 
		/// </summary>
		public bool IsServiceRequest;

		///// <summary>
		///// 
		///// </summary>
		//public ServiceContext ServiceContext;
	}

}
