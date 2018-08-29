using System;
using RpcLite.Config;
using ServiceTest.ServiceImpl;

namespace ServiceTest.WebHost
{
	public class Global : System.Web.HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			RpcInitializer.Initialize(builder => builder
				.AddService<ProductService>("ProductService", "api/service/")
				.AddService<TestService>("ProductService", "api/test/")
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