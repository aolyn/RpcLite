using System.Threading.Tasks;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IInvoker
	{
		/// <summary>
		/// 
		/// </summary>
		string Address { get; set; }

		/// <summary>
		/// service name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// service group
		/// </summary>
		string Group { get; }

		/// <summary>
		/// optional initialize method
		/// </summary>
		/// <param name="name">service name</param>
		/// <param name="group">service group</param>
		void Initialize(string name, string group);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		Task InvokeAsync(ClientContext request);
	}

}
