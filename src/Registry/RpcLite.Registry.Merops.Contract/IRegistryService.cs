using System.Threading.Tasks;

namespace RpcLite.Registry.Merops.Contract
{
	public interface IRegistryService
	{
		GetServiceAddressResponse GetServiceAddress(GetServiceAddressRequest request);

		Task<GetServiceAddressResponse> GetServiceAddressAsync(GetServiceAddressRequest request);
	}
}