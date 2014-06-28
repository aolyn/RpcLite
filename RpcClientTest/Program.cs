using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using ClientImpl;
using Model;
using RpcLite;
using Contracts;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.IO;
using RpcLite.Client;

namespace WebApiClient
{
	class Program
	{
		static void Main(string[] args)
		{

			//{
			//	var ex = new Exception("TEST");
			//	var je = JsonConvert.SerializeObject(ex);
			//	var jo = JsonConvert.DeserializeObject<Exception>(je);

			//	var exbs = Encoding.UTF8.GetBytes(je);
			//	var msEx = new MemoryStream(exbs);
			//	TextReader sr = new StringReader(je);
			//	sr = new StreamReader(msEx);
			//	var jr = new JsonTextReader(sr);
			//	var jc = new JsonSerializer();
			//	var o = jc.Deserialize(jr, ex.GetType());

			//	je = "{\"Message\":\"An error has occurred.\",\"ExceptionMessage\":\"TEST EXCEPTION\",\"ExceptionType\":\"System.Exception\",\"StackTrace\":\"   at WebApiTest.ProductController.Get(Int32 id) in e:\\Documents\\Visual Studio 2010\\Projects\\WebApiTest\\WebApiTest\\Controllers\\ProductsController.cs:line 38\r\n   at lambda_method(Closure , Object , Object[] )\r\n   at System.Web.Http.Controllers.ReflectedHttpActionDescriptor.ActionExecutor.<>c__DisplayClass13.<GetExecutor>b__c(Object instance, Object[] methodParameters)\r\n   at System.Web.Http.Controllers.ReflectedHttpActionDescriptor.ActionExecutor.Execute(Object instance, Object[] arguments)\r\n   at System.Web.Http.Controllers.ReflectedHttpActionDescriptor.<>c__DisplayClass5.<ExecuteAsync>b__4()\r\n   at System.Threading.Tasks.TaskHelpers.RunSynchronously[TResult](Func`1 func, CancellationToken cancellationToken)\"}";
			//	var ser = new JavaScriptSerializer();
			//	var jo2 = JsonConvert.DeserializeObject(je);
			//}
			////var si = new ServiceImpl.ProductService();
			//var rpcConfig = RpcLite.RpcLiteConfigSection.Instance;
			////AssemblyBuilderDemo.Main22();
			//var services = RpcLite.RpcLiteConfigSection.Instance.Services;


			ClientTest();

			//DynamicProxyTest.TestClass.Test();

			//TestDynamicType();

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
				HttpResponseMessage response = client.GetAsync(url).Result;  // Blocking call（阻塞调用）! 
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

		private static void ClientTest()
		{
			{
				var client = new ProductClient();

			}
			{
				var client = RpcClientBase<IProduct>.CreateInstance();
			}
			//client.AddProduct(2, new Product(), 4, 5, 6, 7);
			{

				//RpcLite.NetChannelHelper.GetResponse("Hello", 11);
				//var type = ClientWrapper.WrapInterface<IProduct>();

				var client2 = RpcClientBase<IProduct>.CreateInstance();
				//var obj = RpcClientBase<IProduct>.CreateInstance();
				var ip = client2 as IProduct;
				client2.BaseUrl = "http://localhost:4098/api/";

				var p1 = new Product { Id = 2, Name = "Chris" };
				var result = ip.AddProduct(1, p1, 5, 6, 7, 8);
				var r2 = ip.GetById(1);
				ip.Delete(1);
				var ps1 = ip.Get();
				var ar11 = ip.AddProduct(1, p1);
				var arr2223 = ip.Add(p1);

				return;
			}

			//{

			//	var tw = new WrapperHelper<IProduct>(new object());

			//	var type = tw.GetWrapperType();
			//	var obj = Activator.CreateInstance(type);
			//	var ip = obj as IProduct;
			//}

		}

		private static object Test22()
		{
			return null;
		}

		public static void TestDynamicType()
		{
			//新类型的名称：随便定一个
			const string newTypeName = "Imp_" + "MMTT";

			var propeties = new List<PropertyItemInfo>
			{
				new PropertyItemInfo {Name = "Name", Type = typeof (string)},
				new PropertyItemInfo {Name = "Age", Type = typeof (int)}
			};

			var type = TypeCreator.CreateType(newTypeName, propeties);
			var obj = Activator.CreateInstance(type);
		}


	}
}