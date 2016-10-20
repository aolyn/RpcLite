using System.Linq;
using ServiceRegistry.Dal.EF;

namespace ServiceRegistry.Dal
{
	public class ServiceDal
	{
		internal static string GetServiceAddress(string name, string group)
		{
			using (var ctx = new ServiceRepositoryEntities())
			{
				var mapping = ctx.ServiceProviders
					.FirstOrDefault(it => it.Service.Name == name
						&& it.Group == group);

				return mapping?.Address;
			}
		}
	}
}