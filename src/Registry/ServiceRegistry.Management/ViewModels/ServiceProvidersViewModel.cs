using System.Collections.Generic;
using ServiceRegistry.Domain.Model;

namespace ServiceRegistry.Management.ViewModels
{
	public class ServiceProvidersViewModel
	{
		public IList<ServiceProvider> ServiceProviers { get; internal set; }
		public int ServiceId { get; set; }
	}
}
