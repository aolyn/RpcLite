using ServiceRegistry.Domain.Model;
using ServiceRegistry.Domain.Repositories;

namespace ServiceRegistry.Repositories
{
	public class ServiceProviderRepository : Repository<ServiceProvider, int>, IServiceProviderRepository
	{
	}
}
