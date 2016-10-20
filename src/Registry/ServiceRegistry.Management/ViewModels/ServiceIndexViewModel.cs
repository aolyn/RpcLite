using System.Collections.Generic;
using ServiceRegistry.Domain.Model;

namespace ServiceRegistry.Management.ViewModels
{
	public class ServiceIndexViewModel
	{
		public IList<Service> Services { get; set; }
	}
}
