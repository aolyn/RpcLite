﻿using System;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using RpcLite.Server.Kestrel;
using Xunit;

namespace ServiceTest.UnitTests.SelfHost
{
	public class SelfHostSimpleTest
	{
		[Fact]
		public void Test()
		{
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseRpcLite(config => config.AddService<Service1>("api/service/"))
				.Build();
			host.Start();
			host.StopAsync().GetAwaiter().GetResult();
		}

		public class Service1
		{
			public string GetDateTime()
			{
				return DateTime.Now.ToString(CultureInfo.InvariantCulture);
			}
		}
	}
}
