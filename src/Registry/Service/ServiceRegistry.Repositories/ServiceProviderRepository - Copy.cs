using ServiceRegistry.Domain.Model;
using ServiceRegistry.Domain.Repositories;

namespace ServiceRegistry.Repositories
{
	public class ServiceRepository : Repository<Service, int>, IServiceRepository
	{
	}
}
