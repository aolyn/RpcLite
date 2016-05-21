using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Model;
using Newtonsoft.Json;
using RpcLite.Client;

namespace WebApiClient
{
	class Program
	{
		static void Main(string[] args)
		{
			{
				var client = new WebClient();
				client.Proxy = new WebProxy("http://localhost:8888");
				var html = client.DownloadString("http://mic:37330/");
				//html = client.DownloadString("http://127.0.0.1:37330/");
				html = client.DownloadString("https://www.baidu.com/");

				//var request = (HttpWebRequest)WebRequest.Create("https://www.baidu.com/");
				var request = (HttpWebRequest)WebRequest.Create("http://localhost:37330/");
				request.Proxy = new WebProxy("http://localhost:8888");
				var iar = request.BeginGetResponse(ar =>
				{
					var result = request.EndGetResponse(ar);
					Console.WriteLine(result.ContentLength);
				}, null);
				//var resp = request.GetResponse();
				Console.ReadLine();
			}

			{
				var html = new WebClient().DownloadString("https://www.baidu.com/");

				var request = (HttpWebRequest)WebRequest.Create("https://www.baidu.com/");

				var resp = request.GetResponse();
			}

			{
				var inEx = new ArgumentOutOfRangeException("args111");

				var ex = new AggregateException(new ArgumentException("args", inEx), new NullReferenceException("a is null"));
				var str = JsonConvert.SerializeObject(ex);

				var dExObj = (AggregateException)JsonConvert.DeserializeObject(str, typeof(AggregateException));

				var ex1 = dExObj.InnerExceptions[0];
				var ex1Type = ex1.GetType();

				var ex2 = dExObj.InnerExceptions[1];
				var ex2Type = ex2.GetType();
			}

			if (false)
			{
				//var types = new[]
				//{
				//	//typeof (Ctrip.API.Cruise.H5.Contract.GetShipPOIHomeResponseType),
				//	//typeof (Ctrip.API.Cruise.H5.Contract.GetShipCategoryPOIResponseType),
				//	//typeof (Ctrip.API.Cruise.H5.Contract.GetShipFacilityPOIResponseType),
				//	//typeof (Ctrip.API.Cruise.H5.Contract.GetShipServicePOIResponseType),
				//	//typeof (Ctrip.API.Cruise.H5.Contract.GetCompanyPOIResponseType)

				//	typeof (Ctrip.API.Cruise.H5.Contract.GetSearchItemResponseType),
				//	typeof (Ctrip.API.Cruise.H5.Contract.GetShipCategoryPOIResponseType),
				//	typeof (Ctrip.API.Cruise.H5.Contract.GetShipFacilityPOIResponseType),
				//	typeof (Ctrip.API.Cruise.H5.Contract.GetShipServicePOIResponseType),
				//	typeof (Ctrip.API.Cruise.H5.Contract.GetCompanyPOIResponseType)
				//};

				//foreach (var type in types)
				//{
				//	var obj = SerializationTest.CreateObject(type);
				//	var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
				//	json = json.Replace("\"", "'");
				//	File.WriteAllText(string.Format("c:\\{0}.txt", type.Name), json);
				//}
			}

			//ClientTestForResolverConcurrency();
			ClientTest();
			//ClientTest2();
		}

		private static void ClientTestForResolverConcurrency()
		{
			{
				var task1 = Task.Factory.StartNew(() => { var client = RpcClientBase<IProduct>.GetInstance(); });

				var task2 = Task.Factory.StartNew(() => { var client = RpcClientBase<IProduct>.GetInstance(); });

				Task.WaitAll(task1, task2);
			}
			{
				//var client = new ProductClient();
			}

			{
				var client = RpcClientBase<IProduct>.GetInstance();
			}

			//client.AddProduct(2, new Product(), 4, 5, 6, 7);
			{
				//RpcLite.NetChannelHelper.GetResponse("Hello", 11);
				//var type = ClientWrapper.WrapInterface<IProduct>();

				var client2 = RpcClientBase<IProduct>.GetInstance("http://localhost:37330/api/");
				client2.BaseUrl = "http://localhost:37330/api/";

				client2 = RpcClientBase<IProduct>.GetInstance();

				var ip = client2 as IProduct;

				var p1 = new Product { Id = 2, Name = "Chris" };
				var result = ip.AddProduct(1, p1, 5, 6, 7, 8);
				var r2 = ip.GetById(1);
				ip.Delete(1);
				var ps1 = ip.Get();
				var ar11 = ip.AddProduct(1, p1);
				var arr2223 = ip.Add(p1);
			}

			//{

			//	var tw = new WrapperHelper<IProduct>(new object());

			//	var type = tw.GetWrapperType();
			//	var obj = Activator.CreateInstance(type);
			//	var ip = obj as IProduct;
			//}
		}

		private static void ClientTest()
		{
			{
				var apiBaseUrl = "http://localhost:37330/api/async-product/";

				var client2 = RpcClientBase<IProductAsync>.GetInstance(apiBaseUrl);
				client2.BaseUrl = apiBaseUrl;

				var client332 = ClientFactory.GetInstance<IProductAsync>(apiBaseUrl);

				var ip = client2 as IProductAsync;

				//var html = ip.GetHtml("http://www.baidu.com").Result;
				try
				{
					var html = ip.GetHtml("http://www.baidu.com").Result;
				}
				catch (Exception ex)
				{
					var exType = ex.GetType();
					if (ex.InnerException != null)
					{
						var type = ex.InnerException.GetType();
					}
				}
				//var html2 = ip.GetHtml("http://www.baiduA54sf4we.com").Result;

				//var p1 = new Product { Id = 2, Name = "Chris" };
				//var result = ip.AddProduct(1, p1);
			}

			{
				//var client = new ProductClient();
			}

			{
				var client = RpcClientBase<IProduct>.GetInstance();
			}

			//client.AddProduct(2, new Product(), 4, 5, 6, 7);
			{
				//RpcLite.NetChannelHelper.GetResponse("Hello", 11);
				//var type = ClientWrapper.WrapInterface<IProduct>();

				var apiBaseUrl = "http://localhost:37330/api/product/";

				var client2 = RpcClientBase<IProduct>.GetInstance(apiBaseUrl);
				client2.BaseUrl = apiBaseUrl;

				//client2 = RpcClientBase<IProduct>.GetInstance();

				var ip = client2 as IProduct;

				try
				{
					var p1 = new Product { Id = 2, Name = "Chris" };
					var result = ip.AddProduct(1, p1, 5, 6, 7, 8);
					var r2 = ip.GetById(1);
					ip.Delete(1);
					var ps1 = ip.Get();
					var ar11 = ip.AddProduct(1, p1);
					var arr2223 = ip.Add(p1);
				}
				catch (Exception ex)
				{
					throw;
				}
			}

			//{
			//	var tw = new WrapperHelper<IProduct>(new object());
			//	var type = tw.GetWrapperType();
			//	var obj = Activator.CreateInstance(type);
			//	var ip = obj as IProduct;
			//}
		}

		private static void ClientTest2()
		{
			var str1 = DateTime.Now + "-hh";
			var str2 = DateTime.Now.ToString() + "-hh";

			//var baseAddress = "http://mic:60712/";
			var baseAddress = "http://192.168.9.1:60712/";
			//var baseAddress = "http://localhost:65431/";
			var contentType = "application/json";
			var apiUrl = "api/product";

			{
				var c = new WebApiClient(baseAddress, apiUrl, contentType);
				var ps = c.Get<Product[]>();
				var gizmo = new Product() { Id = 9, Name = "Gizmo", Price = 100, Category = "Widget" };
				var pr = c.Post<Product, int>(gizmo);
				c.Delete<int>(3);
			}

			//{
			//     var ps = new DAL.ProductService(baseAddress, apiUrl, contentType);
			//     var products = ps.Get();
			//     var p1 = ps.Get(1);
			//     var gizmo = new Product() { Id = 9, Name = "Gizmo", Price = 100, Category = "Widget" };
			//     var rv1 = ps.AddProduct(gizmo);
			//}

			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(baseAddress);
			// AddProduct an Accept header for JSON format.
			// 为JSON格式添加一个Accept报头
			//client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

			var oo2 = client.GetAsync(apiUrl).Result;
			//test DELETE
			var drrr = client.DeleteAsync(apiUrl + "/11").Result;
			var drrr2 = client.DeleteAsync(apiUrl).Result;


			{
				// List all products.
				// 列出所有产品
				var url = apiUrl;
				url += "/11";
				HttpResponseMessage response = client.GetAsync(url).Result; // Blocking call（阻塞调用）! 
				if (response.IsSuccessStatusCode)
				{
					// Parse the response body. Blocking!
					// 解析响应体。阻塞！
					var products = response.Content.ReadAsAsync<Product[]>().Result;
					foreach (var p in products)
					{
						//Console.WriteLine("{0}\t{1};\t{2}", p.Name, p.Price, p.Category);
					}
				}
				else
				{
					var content = response.Content.ReadAsByteArrayAsync().Result;
					var contentStr = Encoding.UTF8.GetString(content);
					//var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Exception>(contentStr);
					var products = response.Content.ReadAsAsync<Exception>().Result;

					Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
				}
			}

			{
				// Create a new product
				// 创建一个新产品
				var gizmo = new Product { Name = "Gizmo", Price = 100, Category = "Widget" };
				Uri gizmoUri = null;

				var tsk = client.PostAsJsonAsync("api/product", gizmo);

				var response = client.PostAsJsonAsync("api/product", gizmo).Result;
				if (response.IsSuccessStatusCode)
				{
					var rv = response.Content.ReadAsAsync<int>().Result;
					gizmoUri = response.Headers.Location;
				}
				else
				{
					Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
				}
			}
		}
	}
}