using System;
using RpcLite.Config;
using RpcLite.Registry;
using RpcLite.Registry.Zookeeper;
using Xunit;

namespace ServiceTest.ClientTest
{
	public class ZookeeperRegistryTest
	{
		private readonly ZookeeperRegistry _registry;

		public ZookeeperRegistryTest()
		{
			_registry = new ZookeeperRegistry("192.168.9.1:2181", 100 * 1000);
		}

		[Fact]
		public void Test()
		{
			try
			{
				var lookupTask = _registry.LookupAsync(new ClientConfigItem
				{
					Name = "ProductService",
					Group = "UAT",
				});

				Console.WriteLine("started lookup");
				Console.ReadLine();

				lookupTask.Wait();
				Console.WriteLine("lookupTask.Wait() " + lookupTask.Result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}

			Console.ReadLine();
			_registry.Dispose();
		}

		[Fact]
		public void LookupTest()
		{
			try
			{
				var lookupTask = _registry.LookupAsync(new ClientConfigItem
				{
					Name = "ProductService",
					Group = "UAT",
				});

				Console.WriteLine("started lookup");
				Console.ReadLine();

				lookupTask.Wait();
				Console.WriteLine("lookupTask.Wait() " + lookupTask.Result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		[Fact]
		public void RegisterTest()
		{
			if (_registry.CanRegister)
			{
				_registry.RegisterAsync(new ServiceInfo
				{
					Name = "ProductService",
					Group = "UAT",
					Address = "http://localhost:5000/api/product/",
				}).Wait();
			}

			Console.WriteLine("register finished");
		}
	}
}
