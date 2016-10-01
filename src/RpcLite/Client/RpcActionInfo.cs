using System;
using System.Reflection;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcActionInfo
	{
		#region public properties

		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public MethodInfo MethodInfo { get; set; }

		/// <summary>
		/// result type of action
		/// <para>return type is void, it's null</para>
		/// <para>return type is not task, it's return type</para>
		/// <para>return type of action is Task of T, it's T</para>
		/// </summary>
		public Type ResultType { get; internal set; }

		/// <summary>
		/// 
		/// </summary>
		public int ArgumentCount { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Type ArgumentType { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool HasReturnValue { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsTask { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Type TaskResultType { get; set; }

		/// <summary>
		/// default value or argument type
		/// </summary>
		public object DefaultArgument { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Type ContractType { get; internal set; }

		#endregion

	}
}
