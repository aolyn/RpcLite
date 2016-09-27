using System;
using RpcLite.AspNet;
using ServiceTest.ServiceImpl;

namespace ServiceTest.WebHost
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
			//RpcInitializer.Initialize();
			RpcInitializer.Initialize(builder =>
				builder
					.UseService<ProductService>("ProductService", "api/service/")
					.UseService<TestService>("ProductService", "api/test/")
					.UseServicePaths("api/")
			);
		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{

		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}