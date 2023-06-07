using System;

namespace RpcLite.Filter
{
	/// <summary>
	/// used to subscribe event of service type added to ServiceHost
	/// </summary>
	public interface IServiceTypeAware
	{
		/// <summary>
		/// invoked when service type added to ServiceHost
		/// </summary>
		/// <param name="serviceType"></param>
		void OnServiceTypeAdded(Type serviceType);
	}
}
