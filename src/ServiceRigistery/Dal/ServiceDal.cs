using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceRegistery.Dal
{
	public class ServiceDal
	{
		internal static string GetServiceAddress(string name, string nameSpace, string environment)
		{
			using (var ctx = new ServiceRepositoryEntities())
			{
				var mapping = ctx.ServiceMapping
					.FirstOrDefault(it => it.Service.Name == name
						&& it.Environment == environment
						&& it.Namespace == nameSpace);

				return mapping != null
					? mapping.Address
					: null;
			}
		}
	}
}