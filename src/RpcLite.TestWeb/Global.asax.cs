﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using RpcLite;
using ServiceImpl;

namespace RpcLite.TestWeb
{
	public class Global : System.Web.HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			//RpcHandler.Services.Add(new ServiceInfo { Name = "Products", Path = "~/api/products/", Type = typeof(ProductService) });
			//RpcHandler.Services.Add(new ServiceInfo { Name = "Products", Path = "~/api/", Type = typeof(ProductService) });
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