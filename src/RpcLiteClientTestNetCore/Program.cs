using System;
using System.IO;
using System.Reflection;
using RpcLite;
using RpcLite.Client;
using RpcLite.Config;
using ServiceRegistry.Contract;

namespace RpcLiteClientTestNetCore
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				RpcLiteInitializer.Initialize(null, basePath);

				var client = ClientFactory.GetInstance<IRegistryService>();

				var resp = client.Client.GetServiceAddress(new GetServiceAddressRequest
				{
					ServiceName = "ProductService",
					Namespace = "v1",
					Environment = "IT",
				});

				Console.WriteLine(resp.ToString());
			}
			catch (ConnectionException ex)
			{
				Console.WriteLine(ex);
				//throw;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				//throw;
			}
		}
	}
}
