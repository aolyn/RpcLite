using System.Linq;
using System.Threading.Tasks;
using RpcLite.Registry.Merops.Contract;
using ServiceRegistry.Dal;

namespace ServiceRegistry.Services
{
	public class RegistryService : IRegistryService
	{
		public GetServiceInfoResponse GetServiceInfo(GetServiceInfoRequest request)
		{
			if (request == null)
				return new GetServiceInfoResponse();

			var response = new GetServiceInfoResponse
			{
				Services = request.Services
					.Select(it => new ServiceResultDto
					{
						Identifier = it,
						ServiceInfos = ServiceDal.GetServiceAddresses(it.Name, it.Group)
							.Select(sp => new ServiceInfoDto
							{
								Name = it.Name,
								Group = it.Group,
								Address = sp.Address,
								Data = sp.Data,
							})
							.ToArray(),
					})
					.ToArray(),
			};

			return response;
		}

		public Task<GetServiceInfoResponse> GetServiceInfoAsync(GetServiceInfoRequest request)
		{
			return Task.FromResult(GetServiceInfo(request));
		}
	}
}