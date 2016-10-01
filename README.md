# RpcLite开发指南

----

## 概述

    RpcLite是一个轻量级、易于使用、可扩展、跨平台的SOA框架。
    
    SOA最基本的功能就是提供远程服务调用，在.Net平台下用得比较多的方案有WebService、WCF、ServiceStack、WebApi等。这些方案都只包括RPC，相对这些方案RpcLite还提供了治理系统、监控、负载均衡、多种传输协议支持。
    
    RpcLite在设计时充分考虑了扩展性，主要部件都进行了抽象提供了默认的实现，使用者可以根据自己的需求扩展或替换默认部件来实现自定义的功能。
    
    RpcLite支持Full .Net Framework、.Net Core、Mono，在正文中会介绍Full .Net Framework及.Net Core中的使用方式。

## 架构描述

    RpcLite包括服务端和客户端。
    
    RpcLite的入口是AppHost，AppHost包括向外提供服务的ServiceHost及调用其它服务要使用的ClientFactory，此外包括有监控（Monitor）、Invoker、注册服务（Registry）、序列化（Formatter）、过滤器（Filter）等。

## 开始使用（Getting Started）

    要使用RpcLite提供服务或调用其它服务首先要创建AppHost，创建AppHost需要RpcConfig参数。RpcConfig有两种生成方式：从配置文件读取、通过Fluent风格从ConfigBuilder创建。在本指南中主要通过Fluent风格创建。

### 在Full .Net Framework中使用RpcLite

#### Service
> Full .Net Framework中现只支持Host到Asp.Net中。

*创建服务包括以下步骤：*
* 创建Web Application工程
* 添加RpcLite引用
* Web.config添加RpcHttpModule
* 创建服务契约接口
* 通过实现服务契约接口创建服务类
* 在Global.asax中初始化RpcLite

##### 创建Web Application工程
* 打开Visual Studio 2015
* 打开菜单File ‣ New ‣ Project...
* 在左边的菜单中选择 Templates ‣ Visual C# ‣ Web
* 在项目类型中选择 ASP.NET Web Application (.Net Framework)
* 确保目标Framework版本为 .NET Framework 4.0 或更高
* 填写项目名称HelloRpcLiteService点Ok

##### 添加RpcLite引用
> 添加引用有两种方式：直接下载dll然后引用、通过NuGet添加，其中通过NuGet添加简单方便，本文以此方式为例。
> 通过NuGet添加也有两种方式：图形界面或命令行

*命令行*
* 打开菜单Tools ‣ NuGet Package Manager ‣ Package Manager Console
* 运行 Install-Package RpcLite

*图形界面*
* 在Solution Explorer中右击HelloRpcLite，选择Manage NuGet Packages...
* 在NuGet页面中选择Browse Tab页，然后搜索RpcLite
* 在搜索结果中安装RpcLite

##### Web.config添加RpcHttpModule
> 默认情况下添加RpcLite包依赖后Web.Config会自动添加，若已自动添加请忽略本小节的操作
> 在集成管道模式和经典管道模式中添加HttpModule方式不同，本文以现在用得最多的集成管道模式说明

在configuration/system.webServer节点下添加
<add name="RpcLite" type="RpcLite.Service.RpcHttpModule, RpcLite.NetFx" />

完整配置如下：
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
	<system.webServer>
		<modules>
			<add name="RpcLite" type="RpcLite.Service.RpcHttpModule, RpcLite.NetFx" />
		</modules>
	</system.webServer>
</configuration>
```

##### 创建服务契约接口
* 新建类文件IProductService.cs
* 输入以下内容
```
namespace HelloRpcLiteService
{
	public interface IProductService
	{
		string GetDateTimeString();
	}
}
```

##### 通过实现服务契约接口创建服务类
* 新建类文件ProductService.cs
* 输入以下内容
```
using System;

namespace HelloRpcLiteService
{
	public class ProductService : IProductService
	{
		public string GetDateTimeString()
		{
			return DateTime.Now.ToString();
		}
	}
}
```

##### 在Global.asax中初始化RpcLite
* 向工程中添加Global.asax
* 在Application_Start函数中添加初始化代码
```
using System;
using RpcLite.AspNet;

namespace HelloRpcLiteService
{
	public class Global : System.Web.HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			RpcInitializer.Initialize(builder => builder
				.UseService<ProductService>("ProductService", "api/service/")
				.UseServicePaths("api/")
				);
		}
	}
}
```
*说明*
> * UseService<ProductService>("ProductService", "api/service/")是添加一个服务
泛型参数ProductService为服务提供类
第一个参数"ProductService"为服务名
"api/service/"为服务相对于当前WebApplication根的地址，例如WebApplication地址为http://localhost:8080则服务地址为http://localhost:8080/api/service/。若服务部署到虚拟目录下如http://localhost:8080/app1则服务地址为http://localhost:8080/app1/api/service/
> * UseServicePaths("api/")指定服务地址的前缀，以此地址开始的所有Url都会被认为是RpcLite服务，UseService中使用的路必需在ServicePaths中。若没有配置此选项则不能正常访问服务。

##### 运行
> 到此一个RpcLite服务已经创建完成，可运行查看结果。

* F5运行WebApplication，在浏览器中查看地址，假设是http://localhost:11651
* 在浏览器访问http://localhost:11651/api/service/GetDateTimeString，可看到返回的内容是当前日期
* 在浏览器访问http://localhost:11651/api/service/可以看到当前服务的信息，服务名及所有接口名
```
Service Name: ProductService
Actions:
String GetDateTimeString();
```

#### Client
> 前面已经创建好了一个服务，现在我们创建客户端来访问些服务

*创建客户端包括以下步骤：*
* 创建Console工程
* 添加RpcLite引用
* 创建服务契约接口
* 初始化RpcLite
* 创建RpcClient
* 调用服务方法

##### 创建Console Application工程
* 打开Visual Studio 2015
* 打开菜单File ‣ New ‣ Project...
* 在左边的菜单中选择 Templates ‣ Visual C# ‣ Windows
* 在项目类型中选择 Console Application
* 确保目标Framework版本为 .NET Framework 4.0 或更高
* 填写项目名称HelloRpcLiteClient点Ok

##### 添加RpcLite引用

* 在Solution Explorer中右击HelloRpcLite，选择Manage NuGet Packages...
* 在NuGet页面中选择Browse Tab页，然后搜索RpcLite
* 在搜索结果中安装RpcLite

##### 创建服务契约接口
> 直接复制服务中的文件即可，或添加Link引用。
> 在实际开发时契约接口可放到单独的Project中，Service及Client端引用此Project即可。
> 为了简单在此还是创建一个新的文件

* 新建类文件IProductService.cs
* 输入以下内容
```
namespace HelloRpcLiteService
{
	public interface IProductService
	{
		string GetDateTimeString();
	}
}
```

*注*
> 契约接口不要求Service与Client端完全相同，Client端只需定义需要调用的函数即可，名必须与Service端接口中一致。

##### 初始化RpcLite
* 向工程中添加Global.asax
* 在Application_Start函数中添加初始化代码
```
RpcInitializer.Initialize(builder => builder
		.UseClient<IProductService>()
);

```
*说明*
> UseClient<IProductService>()是添加一个服务引用泛型参数IProductService为服务契约接口
> UseClient也有带服务名，及服务地址的重载

##### 创建RpcClient
```
var serviceAddress = "http://localhost:11651/api/service/";
var client = ClientFactory.GetInstance<IProductService>(serviceAddress);
```

##### 调用服务方法
```
var dateTimeString = client.GetDateTimeString();
```

*完整代码如下*
```
using System;
using RpcLite.AspNet;
using RpcLite.Client;

namespace HelloRpcLiteClient
{
	class Program
	{
		static void Main(string[] args)
		{
			RpcInitializer.Initialize(builder => builder
					.UseClient<IProductService>()
			);

			var serviceAddress = "http://localhost:11651/api/service/";
			var client = ClientFactory.GetInstance<IProductService>(serviceAddress);

			try
			{
				var dateTimeString = client.GetDateTimeString();
				Console.WriteLine("DateTime now from service is " + dateTimeString);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			Console.ReadLine();
		}
	}
}
```

* F5运行HelloRpcLiteService
* 在Solution Explorer中选中HelloRpcLiteClient右击，选择菜单Debug > Start new instance
* Console窗口中可看到运行结果
> DateTime now from service is 2016-09-28 00:04:20


### 在.NET Core中使用RpcLite

#### Service

#### Client
