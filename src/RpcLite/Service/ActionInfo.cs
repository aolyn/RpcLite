using System;
using System.Reflection;

namespace RpcLite
{
	public class ActionInfo
	{
		public string Name { get; set; }
		public MethodInfo MethodInfo { get; set; }
		public int ArgumentCount { get; set; }
		public Type ArgumentType { get; set; }
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
		public Func<object, IAsyncResult, object> EndFunc { get; set; }
		public Action<object, object> Action { get; set; }
		public Func<object, object, AsyncCallback, object, IAsyncResult> BeginAction { get; set; }
		//public Func<object, object, object, object, object> BeginAction { get; set; }
		public Action<object, IAsyncResult> EndAction { get; set; }
		public Func<object> ServiceCreator { get; set; }

		public bool IsAsync { get; set; }
	}
}