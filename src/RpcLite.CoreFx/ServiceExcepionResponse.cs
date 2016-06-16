using System;

namespace RpcLite
{
	/// <summary>
	/// 
	/// </summary>
	public class ServiceExcepionResponse
	{
		/// <summary>
		/// 0-success
		/// </summary>
		public int ErrorCode { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string ExceptionType { get; set; }

		/// <summary>
		/// ExceptionAssembly
		/// </summary>
		public string ExceptionAssembly { get; internal set; }

		/// <summary>
		/// Is Framework Execption
		/// </summary>
		public bool IsFrameworkExecption { get; internal set; }

		/// <summary>
		/// 
		/// </summary>
		public Exception InnerException { get; set; }
	}
}
