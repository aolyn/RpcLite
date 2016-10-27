using System.Collections.Generic;
using ServiceRegistry.Domain.Model;

namespace ServiceRegistry.Management.ViewModels
{
	public class ServiceListComponentViewModel
	{
		public string ErrorMessage { get; set; }
		public IList<Service> Services { get; set; }
	}
}
