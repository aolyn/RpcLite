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
		/// <param name="requestPath"></param>
		/// <param name="pathInfo">parsed path info of request path</param>
		/// <returns></returns>
		MapServiceResult MapService(string requestPath, RequestPathInfo pathInfo);
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

		/// <summary>
		/// 
		/// </summary>
		public string ActionName;

		/// <summary>
		/// 
		/// </summary>
		public RpcService Service;

		///// <summary>
		///// 
		///// </summary>
		//public ServiceContext ServiceContext;
	}

}
