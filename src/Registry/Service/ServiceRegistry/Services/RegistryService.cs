using System;
using System.Threading.Tasks;
using ServiceRegistry.Contract;
using ServiceRegistry.Dal;

namespace ServiceRegistry.Services
{
	public class RegistryService : IRegistryService
	{
		public GetServiceAddressResponse GetServiceAddress(GetServiceAddressRequest request)
		{
			if (request == null)
				return new GetServiceAddressResponse();

			var response = new GetServiceAddressResponse
			{
				ServiceName = request.ServiceName,
				//Namespace = request.Namespace,
				Environment = request.Environment,
				Address = ServiceDal.GetServiceAddress(request.ServiceName, request.Environment)
			};

			return response;
		}

		public Task<GetServiceAddressResponse> GetServiceAddressAsync(GetServiceAddressRequest request)
		{
			return Task.FromResult(GetServiceAddress(request));
		}

	}
}