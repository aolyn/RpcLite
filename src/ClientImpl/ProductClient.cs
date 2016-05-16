//using System;
//using System.Collections.Generic;
//using System.Text;
//using Contracts;
//using Model;
//using Newtonsoft.Json;
//using RpcLite.Client;

//namespace ClientImpl
//{
//	class ProductArg1
//	{
//		public int Id { get; set; }
//		public Product Product { get; set; }

//		public void SetValues(int id, Product product)
//		{
//			Id = id;
//			Product = product;
//		}
//	}

//	class ProductArg2
//	{
//		public int Id { get; set; }
//		public Product Product { get; set; }

//		public void SetValues(int id, Model.Product product, int a, int b, int c, int d)
//		{
//		}
//	}

//	class NetHelper
//	{
//		public static object GetResponse(string action, object request)
//		{
//			var strJson = JsonConvert.SerializeObject(request);

//			return null;
//		}
//	}

//	public class ProductClient : RpcClientBase<IProduct> , IProduct
//	{
//		#region IProduct Members

//		public int Add(Model.Product product)
//		{
//			return (int)GetResponse("Add", product, typeof(int));
//		}

//		public int AddProduct(int id, Model.Product product)
//		{
//			var obj = new ProductArg1();
//			obj.SetValues(id, product);
//			return (int)GetResponse("AddProduct", obj, typeof(int));

//			#region

//			//var resp = NetHelper.GetResponse("AddProduct", obj);
//			//return (int)resp;

//			//var resp = NetHelper.GetResponse("AddProduct", obj);
//			//var method = MethodBase.GetCurrentMethod();
//			//var paras = method.GetParameters();
//			//if (paras.Length > 1)
//			//{
//			//     var paramType = RpcProcessor.GetParameterType(method);
//			//     var pi = Activator.CreateInstance(paramType);

//			//     //Expression.ass
//			//}
//			//else if (paras.Length == 1)
//			//{

//			//}


//			//var props = method.GetParameters()
//			//     .Select(it => new PropertyItemInfo { Name = it.Name, Type = it.ParameterType })
//			//     .ToList();

//			//var name = GetType().Name + "." + method.Name;
//			//var paramType = TypeCreator.CreateType(name, props);

//			//throw new NotImplementedException();
//			//return 0;
//			#endregion
//		}

//		public void Delete(int id)
//		{
//			GetResponse("Delete", id, typeof(void));
//		}

//		public Model.Product[] Get()
//		{
//			throw new NotImplementedException();
//		}

//		public Model.Product GetById(int id)
//		{
//			return (Product)GetResponse("GetById", id, typeof(Product));
//		}

//		public Model.Product Get(int id)
//		{
//			throw new NotImplementedException();
//		}

//		#endregion

//		#region IProduct Members

//		private string serviceUrl = null;
//		public int AddProduct(int id, Model.Product product, int a, int b, int c, int d)
//		{
//			var obj = new ProductArg2();
//			obj.SetValues(id, product, a, b, c, d);
//			return (int)NetHelper.GetResponse("AddProduct", obj);
//		}

//		private object DoRequest(Type paramType, string json)
//		{
//			var headDic = new Dictionary<string, string>
//				{
//					{"Content-Type","application/json"},
//					{"Accept","application/json"},
//				};

//			var result = RpcLite.Net.WebRequestHelper.PostData(serviceUrl, json, Encoding.UTF8, headDic);
//			var resultObj = JsonConvert.DeserializeObject(result, paramType);
//			return resultObj;
//		}

//		#endregion
//	}
//}
