using ServiceRegistry.Domain.Model;
using ServiceRegistry.Domain.Repositories;

namespace ServiceRegistry.Repositories
{
	public class ServiceMappingRepository : Repository<ServiceMapping, int>, IServiceMappingRepository
	{
	}
}
