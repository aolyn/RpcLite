using System.Threading.Tasks;
using RpcLite.Formatters;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IInvoker<TContract>
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
		/// <typeparam name="TResult"></typeparam>
		/// <param name="action"></param>
		/// <param name="request"></param>
		/// <param name="actionInfo"></param>
		/// <param name="formatter">prefer formatter</param>
		/// <returns></returns>
		Task<TResult> InvokeAsync<TResult>(string action, object request, RpcActionInfo actionInfo, IFormatter formatter);
	}
}
