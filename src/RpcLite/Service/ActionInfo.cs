using System;
using System.Reflection;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class ActionInfo
	{
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public MethodInfo MethodInfo { get; set; }

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
		/// T: service, argument, return
		/// </summary>
		public Func<object, object, object> Func { get; set; }

		/// <summary>
		/// T: service, argument, callback, state, return
		/// </summary>
		public Func<object, object, AsyncCallback, object, IAsyncResult> BeginFunc { get; set; }

		//public Func<object, object, object, object, object> BeginFunc { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Func<object, IAsyncResult, object> EndFunc { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Action<object, object> Action { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Func<object, object, AsyncCallback, object, IAsyncResult> BeginAction { get; set; }
		//public Func<object, object, object, object, object> BeginAction { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Action<object, IAsyncResult> EndAction { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public Func<object> ServiceCreator { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsAsync { get; set; }

		public bool IsStatic { get; set; }
	}
}