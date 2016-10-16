using System.Collections.Generic;

namespace ServiceRegistry.Domain.Model
{

	public class ServiceGroup : AggregateRoot<string>
	{
		//public string Name { get; set; }

		public virtual List<ServiceProducer> ServiceProducers { get; set; }
	}
}
