
namespace ServiceRegistry.Contract
{
	public interface IRegistryService
	{
		GetServiceAddressResponse GetServiceAddress(GetServiceAddressRequest request);
	}
}