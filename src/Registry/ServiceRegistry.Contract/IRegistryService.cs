using System.Threading.Tasks;

namespace ServiceRegistry.Contract
{
	public interface IRegistryService
	{
		GetServiceAddressResponse GetServiceAddress(GetServiceAddressRequest request);

		Task<GetServiceAddressResponse> GetServiceAddressAsync(GetServiceAddressRequest request);
	}
}