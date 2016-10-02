using System.Threading.Tasks;
using RpcLite;
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

			using (var repository = new ServiceMappingRepository())
			{
				var serviceMapping = repository.GetAsync(it => it.Service.Name == request.ServiceName
						//&& it.Namespace == request.Namespace
						&& it.Environment == request.Group).Result;

				var response = new GetServiceAddressResponse
				{
					ServiceName = request.ServiceName,
					//Namespace = request.Namespace,
					Group = request.Group,
					Address = serviceMapping?.Address,
				};

				return response;
			}
		}

		public Task<GetServiceAddressResponse> GetServiceAddressAsync(GetServiceAddressRequest request)
		{
			if (request == null)
				return TaskHelper.FromResult(new GetServiceAddressResponse());

			var repository = new ServiceMappingRepository();
			var serviceMappingTask = repository.GetAsync(it => it.Service.Name == request.ServiceName
					//&& it.Namespace == request.Namespace
					&& it.Environment == request.Group);

			return serviceMappingTask.ContinueWith(tsk =>
			{
				var response = new GetServiceAddressResponse
				{
					ServiceName = request.ServiceName,
					//Namespace = request.Namespace,
					Group = request.Group,
					Address = tsk.Result?.Address,
				};

				return response;

			});

		}
	}
}