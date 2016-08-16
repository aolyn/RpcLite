using System.IO;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>

	public interface IServerContext
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		string GetRequestHeader(string key);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		void SetResponseHeader(string key, string value);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		string GetResponseHeader(string key);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		void SetResponseContentType(string type);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string GetResponseContentType();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string GetRequestContentType();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		Stream GetRequestStream();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		Stream GetResponseStream();

		/// <summary>
		/// 
		/// </summary>
		string RequestPath { get; }

		/// <summary>
		/// 
		/// </summary>
		int RequestContentLength { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="statusCode"></param>
		void SetResponseStatusCode(int statusCode);
	}
}
