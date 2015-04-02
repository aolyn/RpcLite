
namespace ServiceRegistery.Contract
{
	public interface IRegistryService
	{
		GetServiceAddressResponse GetServiceAddress(GetServiceAddressRequest request);
	}
}