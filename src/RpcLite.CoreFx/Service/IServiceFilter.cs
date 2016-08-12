using System;
using System.Threading.Tasks;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public interface IServiceFilter
	{
		/// <summary>
		/// 
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// if filter invoke must call next in Invoke 
		/// </summary>
		bool FilterInvoke { get; }

		/// <summary>
		/// if filter invoke must call next in Invoke 
		/// </summary>
		Task Invoke(ServiceContext context, Func<ServiceContext, Task> next);

		void BeforeInvoke(ServiceContext context);

		void AfterInvoke(ServiceContext context);
	}
}
