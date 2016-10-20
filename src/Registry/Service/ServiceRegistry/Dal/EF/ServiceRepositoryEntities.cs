using System.Data.Entity;
using ServiceRegistry.Domain.Model;

namespace ServiceRegistry.Dal.EF
{
	public class ServiceRepositoryEntities : DbContext
	{
		public DbSet<Service> Services { get; set; }
		public DbSet<ServiceGroup> ServiceGroups { get; set; }
		public DbSet<ServiceProvider> ServiceProviders { get; set; }
	}
}