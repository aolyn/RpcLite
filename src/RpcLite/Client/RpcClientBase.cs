//#define LogDuration
#if DEBUG && LogDuration
using System.Diagnostics;
#endif

using System;
using System.Threading.Tasks;
using RpcLite.Formatters;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TContract">contract interface</typeparam>
	public class RpcClientBase<TContract> : IRpcClient<TContract>
		where TContract : class
	{
		private readonly ClientActionManager _actionManager;

		/// <summary>
		/// 
		/// </summary>
		public RpcClientBase()
		{
			_actionManager = ClientActionManager.GetInstance(null, typeof(TContract));
		}

		/// <summary>
		/// base url of service
		/// </summary>
		public string Address
		{
			get { return Invoker?.Address; }
			set
			{
				if (Invoker != null)
					Invoker.Address = value;
			}
		}

		//private IFormatter _formatter = new JsonFormatter();
		/// <summary>
		/// Formatter
		/// </summary>
		public IFormatter Formatter { get; set; }

		/// <summary>
		/// get Name of Formatter or set Formatter by Name
		/// </summary>
		public string Format
		{
			get { return Formatter?.Name; }
			set
			{
				if (AppHost == null)
					throw new InvalidOperationException("when AppHost is null, only Formatter can be set");

				var formatter = AppHost.FormatterManager?.GetFormatterByName(value);
				if (formatter == null)
					throw new FormatterNotFoundException(value);
				Formatter = formatter;
			}
		}

		/// <summary>
		/// Channel to transport data with service
		/// </summary>
		public IInvoker<TContract> Invoker { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="request"></param>
		/// <param name="argumentType"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		protected object GetResponse<TResult>(string action, object request, Type argumentType, Type returnType)
		{
#if DEBUG && LogDuration
			var stopwatch = Stopwatch.StartNew();
#endif
			var response = GetResponseAsync<TResult>(action, request, argumentType, returnType);
			try
			{
#if DEBUG && LogDuration
				stopwatch.Stop();
				var duration = stopwatch.ElapsedMilliseconds;
#endif
				var result = response.Result;
				return result;
			}
			catch (AggregateException ex)
			{
				throw ex.InnerException;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="request"></param>
		/// <param name="argumentType"></param>
		/// <param name="returnType"></param>
		/// <returns></returns>
		protected Task<TResult> GetResponseAsync<TResult>(string action, object request, Type argumentType, Type returnType)
		{
			if (Formatter == null)
				throw new ServiceException("Formatter can't be null");

			var actionInfo = _actionManager.GetAction(action);
			var resultObj = Invoker.InvokeAsync<TResult>(action, request, actionInfo, Formatter);
			return resultObj;
		}

		/// <summary>
		/// 
		/// </summary>
		internal AppHost AppHost { get; set; }

		/// <summary>
		/// service name
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// service group
		/// </summary>
		public string Group { get; internal set; }
	}
}
