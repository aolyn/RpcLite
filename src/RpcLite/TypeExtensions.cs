using System.Reflection;

// ReSharper disable once CheckNamespace
namespace System
{
	/// <summary>
	/// 
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static
#if NETCORE
			TypeInfo
#else
			Type
#endif
			GetTypeInfoEx(this Type type)
		{
#if NETCORE
			return type.GetTypeInfo();
#else
			return type;
#endif
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="source"></param>
		/// <returns></returns>
		public static bool IsAssignableFromEx(this Type type, Type source)
		{
#if NETCORE
			return type.GetTypeInfoEx()
				.IsAssignableFrom(source.GetTypeInfoEx());
#else
			return type.IsAssignableFrom(source);
#endif
		}

	}
}
