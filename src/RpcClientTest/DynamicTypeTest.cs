using System;
using System.Collections.Generic;
using RpcLite;

namespace WebApiClient
{
	class DynamicTypeTest
	{
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
