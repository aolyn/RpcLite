using ServiceRegistery.Contract;
using ServiceRegistery.Dal;

namespace ServiceRegistery.Services
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
				Namespace = request.Namespace,
				Environment = request.Environment,
				Address = ServiceDal.GetServiceAddress(request.ServiceName, request.Namespace, request.Environment)
			};

			return response;
		}
	}
}