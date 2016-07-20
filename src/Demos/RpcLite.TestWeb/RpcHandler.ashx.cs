using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceImpl;

namespace RpcLite
{
	public class RpcHandler : IHttpHandler
	{
		public class ServiceInfo
		{
			public string Path { get; set; }
			public Type Type { get; set; }
			public string Name { get; set; }
		}

		public static List<ServiceInfo> Services = new List<ServiceInfo>();
		static RpcHandler()
		{
			Services.Add(new ServiceInfo { Name = "Products", Path = "~/api/products/", Type = typeof(ProductService) });
			Services.Add(new ServiceInfo { Name = "Products", Path = "~/api/", Type = typeof(ProductService) });
		}

		public void ProcessRequest(HttpContext context)
		{
			var request = context.Request;
			var response = context.Response;
			var service =
				Services.FirstOrDefault(
					it => request.AppRelativeCurrentExecutionFilePath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));

			if (service != null)
			{
				var actionName = request.AppRelativeCurrentExecutionFilePath.Substring(service.Path.Length);
				RpcProcessor.ProcessRequest(service.Type, actionName, request, response);
			}

			context.Response.ContentType = "text/plain";
			context.Response.Write("Hello World");
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}