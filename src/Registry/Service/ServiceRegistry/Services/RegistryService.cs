using System;
using System.Threading.Tasks;
using RpcLite.Registry.Merops.Contract;
using ServiceRegistry.Dal;

namespace ServiceRegistry.Services
{
	public class RegistryService : IRegistryService
	{
		public GetServiceInfoResponse GetServiceInfo(GetServiceInfoRequest request)
		{
			throw new NotImplementedException();
			//if (request == null)
			//	return new GetServiceInfoResponse();

			//var response = new GetServiceInfoResponse
			//{
			//	ServiceName = request.ServiceName,
			//	//Namespace = request.Namespace,
			//	Group = request.Group,
			//	Address = ServiceDal.GetServiceInfo(request.ServiceName, request.Group)
			//};

			//return response;
		}

		public Task<GetServiceInfoResponse> GetServiceInfoAsync(GetServiceInfoRequest request)
		{
			return Task.FromResult(GetServiceInfo(request));
		}

	}
}