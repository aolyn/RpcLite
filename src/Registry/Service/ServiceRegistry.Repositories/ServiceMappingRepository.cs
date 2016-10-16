using ServiceRegistry.Domain.Model;
using ServiceRegistry.Domain.Repositories;

namespace ServiceRegistry.Repositories
{
	public class ServiceMappingRepository : Repository<ServiceProducer, int>, IServiceMappingRepository
	{
	}
}
