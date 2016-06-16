using ServiceRegistry.Contract;
using ServiceRegistry.Repositories;

namespace ServiceRegistry.Service.Services
{
	public class RegistryService : IRegistryService
	{
		public GetServiceAddressResponse GetServiceAddress(GetServiceAddressRequest request)
		{
			if (request == null)
				return new GetServiceAddressResponse();

			var repository = new ServiceMappingRepository();
			var serviceMapping = repository.GetAsync(it => it.Service.Name == request.ServiceName
					&& it.Namespace == request.Namespace
					&& it.Environment == request.Environment).Result;

			var response = new GetServiceAddressResponse
			{
				ServiceName = request.ServiceName,
				Namespace = request.Namespace,
				Environment = request.Environment,
				Address = serviceMapping?.Address,
			};

			return response;
		}
	}
}