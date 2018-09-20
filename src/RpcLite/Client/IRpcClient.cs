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
		/// name for Formatter,
		/// <para>get: return Formatter.Name</para>
		/// <para>set: find property Formatter set to Formatter field</para>
		/// </summary>
		string Format { get; set; }

		/// <summary>
		/// Invoker
		/// </summary>
		IInvoker Invoker { get; set; }

		/// <summary>
		/// service name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// service group
		/// </summary>
		string Group { get; }
	}

}