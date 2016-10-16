namespace ServiceRegistry.Domain.Model
{
	public class ServiceProducer : AggregateRoot<int>
	{
		public int ServiceId { get; set; }
		public string Group { get; set; }
		public string Address { get; set; }
		public string Data { get; set; }

		public virtual Service Service { get; set; }
		public virtual ServiceGroup ServiceGroup { get; set; }
	}
}
