using System.Threading.Tasks;

namespace RpcLite.Registry.Merops.Contract
{
	public interface IRegistryService
	{
		GetServiceInfoResponse GetServiceInfo(GetServiceInfoRequest request);

		Task<GetServiceInfoResponse> GetServiceInfoAsync(GetServiceInfoRequest request);
	}
}