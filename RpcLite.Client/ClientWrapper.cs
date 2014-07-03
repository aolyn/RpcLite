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
		private static readonly object generateTypeLock = new object();
		private static readonly Dictionary<Type, object> ImplementTypeLocks = new Dictionary<Type, object>();

		/// <summary>
		/// thread safe
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static Type WrapInterface<T>()
		{
			var contractType = typeof(T);
			Type implementType;
			if (ImplementTypes.TryGetValue(contractType, out implementType))
				return implementType;

			object typeLock;
			lock (generateTypeLock)
			{
				if (!ImplementTypeLocks.TryGetValue(contractType, out typeLock))
				{
					typeLock = new object();
					ImplementTypeLocks.Add(contractType, typeLock);
				}
			}

			lock (typeLock)
			{
				if (ImplementTypes.TryGetValue(contractType, out implementType))
					return implementType;

				var parentType = typeof(RpcClientBase<object>).GetGenericTypeDefinition().MakeGenericType(contractType);
				implementType = TypeCreator.WrapInterface<T>(parentType);
				ImplementTypes.Add(contractType, implementType);
				ImplementTypeLocks.Remove(contractType);
				return implementType;
			}

		}
	}
}
