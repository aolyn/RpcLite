using System.Collections.Generic;

namespace ServiceRegistry.Domain.Model
{

	public class ServiceGroup : AggregateRoot<string>
	{
		public virtual List<ServiceProvider> ServiceProviders { get; set; }
	}
}
