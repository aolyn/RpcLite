using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpcLite.Client
{
	internal class ClientWrapper
	{
		private static readonly Dictionary<Type, Type> ImplementTypes = new Dictionary<Type, Type>();
		public static Type WrapInterface<T>()
		{
			var contractType = typeof(T);
			Type implementType;
			if (ImplementTypes.TryGetValue(contractType, out implementType))
				return implementType;

			var parentType = typeof(RpcClientBase<object>).GetGenericTypeDefinition().MakeGenericType(contractType);
			implementType = TypeCreator.WrapInterface<T>(parentType);
			ImplementTypes.Add(contractType, implementType);
			return implementType;
		}
	}
}
