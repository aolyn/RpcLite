using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using RpcLite;
using RpcLite.Client;
using RpcLite.Config;
using RpcLiteTestService.Contract;
using ServiceRegistry.Contract;

namespace RpcLiteClientTestNetCore
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				//var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
				//RpcLiteInitializer.Initialize(null, basePath);
				RpcLiteInitializer.Initialize();

				{
					var url = "http://localhost:5000/api/service/GetDateTimeString";
					var httpClient = new HttpClient();

					for (int i = 0; i < 3; i++)
					{
						var stopwatch = Stopwatch.StartNew();
						//var resp = httpClient.GetAsync(url);

						var msg = new HttpRequestMessage();
						var resp = httpClient.PostAsync(url, new ByteArrayContent(new byte[] { 0 })).Result;

						var d1 = stopwatch.GetAndRest();

						var request = WebRequest.Create(url);
						var response = request.GetResponseAsync().Result;
						var d2 = stopwatch.GetAndRest();

						Console.WriteLine("HttpClient use time in ms " + d1);
						Console.WriteLine("WebRequest use time in ms " + d2);
						Console.WriteLine();
					}
				}

				{
					for (int i = 0; i < 3; i++)
					{
						var client = ClientFactory.GetInstance<ITestService>("http://localhost:5000/api/service/");
						using (new TimeRecorder())
						{
							client.SetAge(1221);
						}

						using (new TimeRecorder())
						{
							client.AddAsync(1, 3).Wait();
						}

						client.SetAgeAsync(1221).Wait();
						using (new TimeRecorder())
						{
							var dateTime = client.GetDateTimeString();
						}
						Console.WriteLine();
					}

					//var testService = client as RpcClientBase<ITestService>;
					//if (testService != null)
					//{
					//	var baseurl = testService.BaseUrl;
					//}
				}

				{
					var client = ClientFactory.GetInstance<IRegistryService>();

					var resp = client.Client.GetServiceAddressAsync(new GetServiceAddressRequest
					{
						ServiceName = "ProductService",
						Namespace = "v1",
						Environment = "IT",
					});

					Console.WriteLine(resp.Result.ToString());

				}
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
