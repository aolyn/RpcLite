using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using RpcLite.Client;
using RpcLite.Config;
using RpcLite.Service;
using ServiceTest.Contract;

namespace ServiceTest.ClientTest
{
	public class UnitTest
	{
		internal static void Test()
		{
			Test1();
		}

		public static void Test1()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("rpclite.config.json")
				.Build();

			var appHost = new AppHost(RpcConfigHelper.GetConfig(new CoreConfiguration(config)));
			appHost.Initialize();

			var clientBuilder = new RpcClientBuilder<IProductService>(appHost.RegistryManager);

			var client = clientBuilder.GetInstance();

			var channel = new MemoryClientChannel() { Address = "/api/service/" };
			channel.ExcuteFunc = (string action, Stream content, IDictionary<string, string> headers) =>
			{
				var outputStream = new MemoryStream();
				var responseHeader = new Dictionary<string, string>();

				IServerContext request = new GenericServerContext()
				{
					RequestContentLength = (int)content.Length,
					RequestContentType = headers["Content-Type"],
					RequestPath = channel.Address + action,
					RequestStream = content,
					RequestHeader = headers,

					ResponseStream = outputStream,
					ResponseHeader = responseHeader,
				};

				var processed = appHost.ProcessAsync(request).Result;
				outputStream.Position = 0;
				var response = new ResponseMessage(null)
				{
					Headers = responseHeader,
					IsSuccess = true,
					Result = outputStream,
				};

				return Task.FromResult(response);
			};

			((IRpcClient)client).Channel = channel;
			var products = client.GetAll();

			var stopwatch = Stopwatch.StartNew();
			var times = 10000;
			for (int i = 0; i < times; i++)
			{
				var products2 = client.GetAll();
			}

			stopwatch.Stop();
			Console.WriteLine($"Elapsed: {stopwatch.Elapsed.TotalMilliseconds}, {times * 1000 / stopwatch.Elapsed.TotalMilliseconds} tps");

			Console.ReadLine();
		}
	}

}
