using System;
using System.Collections.Generic;
using System.IO;

namespace RpcLite.Formatters
{
	/// <summary>
	/// 
	/// </summary>
	public interface IFormatter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputStream"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		object Deserialize(Stream inputStream,Type targetType);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="outputStream"></param>
		/// <param name="source"></param>
		void Serialize(Stream outputStream, object source);

		/// <summary>
		/// 
		/// </summary>
		List<string> SupportMimes { get; }
	}
}
