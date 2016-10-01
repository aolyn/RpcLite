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
