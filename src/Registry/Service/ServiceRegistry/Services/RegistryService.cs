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
			throw new NotImplementedException();
			//if (request == null)
			//	return new GetServiceAddressResponse();

			//var response = new GetServiceAddressResponse
			//{
			//	ServiceName = request.ServiceName,
			//	//Namespace = request.Namespace,
			//	Group = request.Group,
			//	Address = ServiceDal.GetServiceAddress(request.ServiceName, request.Group)
			//};

			//return response;
		}

		public Task<GetServiceAddressResponse> GetServiceAddressAsync(GetServiceAddressRequest request)
		{
			return Task.FromResult(GetServiceAddress(request));
		}

	}
}