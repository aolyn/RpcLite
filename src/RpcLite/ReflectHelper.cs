using System;
using System.Reflection;
#if NETFX
using System.Linq;
#endif

namespace RpcLite
{
	/// <summary>
	/// 
	/// </summary>
	public static class ReflectHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="identifier"></param>
		/// <returns></returns>
		public static T CreateInstanceByIdentifier<T>(string identifier)
		{
			if (string.IsNullOrWhiteSpace(identifier))
				throw new ArgumentNullException(nameof(identifier));

			var type = GetTypeByIdentifier(identifier);
			if (type == null)
				throw new InvalidOperationException($"can not found type \"{identifier}\"");

			return (T)Activator.CreateInstance(type);
		}


		/// <summary>
		/// Get type from assembly
		/// </summary>
		/// <param name="name">Namespace.TypeName, AssemblyName</param>
		/// <returns></returns>
		public static Type GetTypeByIdentifier(string name)
		{
			var segs = name.Split(',');

			var typeName = segs[0];
			var assemblyName = segs[1];
			return GetTypeFromName(typeName, assemblyName);
		}

		/// <summary>
		/// Get type from assembly
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		private static Type GetTypeFromName(string typeName, string assemblyName)
		{
			Assembly assembly;

#if NETCORE
			var asmName = new AssemblyName(assemblyName);
			assembly = Assembly.Load(asmName);
#else
			if (!string.IsNullOrWhiteSpace(assemblyName))
			{
				var asms = AppDomain.CurrentDomain.GetAssemblies();
				assembly = asms.FirstOrDefault(it => it.FullName.StartsWith(assemblyName + ",", StringComparison.OrdinalIgnoreCase))
					?? Assembly.Load(assemblyName);
			}
			else
			{
				assembly = Assembly.GetEntryAssembly();
			}
#endif

			var typeInfo = assembly.GetType(typeName);
			return typeInfo;
		}
	}
}
