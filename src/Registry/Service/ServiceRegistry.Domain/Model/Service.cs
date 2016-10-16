using System.Collections.Generic;

namespace ServiceRegistry.Domain.Model
{
	public class Service : AggregateRoot<int>
	{
		public string Name { get; set; }

		public virtual List<ServiceProducer> ServiceMappings { get; set; }
	}
}
