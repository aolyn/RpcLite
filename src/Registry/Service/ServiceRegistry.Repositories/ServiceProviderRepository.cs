using ServiceRegistry.Domain.Model;
using ServiceRegistry.Domain.Repositories;

namespace ServiceRegistry.Repositories
{
	public class ServiceGroupRepository : Repository<ServiceGroup, string>, IServiceGroupRepository
	{
	}
}
