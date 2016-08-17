using System.Collections.Generic;

namespace ServiceRegistry.Domain.Model
{

	public class ServiceEnvironment : AggregateRoot<string>
	{
		//public string Name { get; set; }

		public virtual List<ServiceMapping> ServiceMappings { get; set; }
	}
}
