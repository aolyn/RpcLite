using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpcLite.Client
{
	public class RpcClientFactory<T> where T : class
	{
		public static T GetInstance()
		{
			var type = ClientWrapper.WrapInterface<T>();
			var obj = Activator.CreateInstance(type);

			return (T)obj;
		}
	}
}
