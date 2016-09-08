namespace ServiceRegistry.Domain.Model
{
	public class ServiceMapping : AggregateRoot<int>
	{
		public int ServiceId { get; set; }
		public string Namespace { get; set; }
		public string Environment { get; set; }
		public string Address { get; set; }

		public virtual Service Service { get; set; }
		public virtual ServiceEnvironment ServiceEnvironment { get; set; }
	}
}
