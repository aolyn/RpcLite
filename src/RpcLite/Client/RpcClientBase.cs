//#define LogDuration
#if DEBUG && LogDuration
using System.Diagnostics;
#endif

using System;
using System.Linq;
using System.Threading.Tasks;
using RpcLite.Formatters;
using RpcLite.Logging;

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
		private IClientInvokeFilter[] _invokeFilters;
		private long _oldFilterVersion = -1;

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
			var monitor = AppHost?.Monitor?.GetClientSession();
			var context = new ClientContext
			{
				Action = actionInfo,
				Request = request,
				Formatter = Formatter,
				Monitor = monitor,
			};
			monitor?.OnInvoking(context);
			ApplyOnInvokingFilter(context);
			var task = Invoker.InvokeAsync<TResult>(context);
			return task.ContinueWith(tsk =>
			{
				ApplyOnInvokedFilter(context);
				monitor?.OnInvoked(context);
				if (tsk.Exception != null)
					throw tsk.Exception.InnerException;
				return tsk.Result;
			});
		}

		private void ApplyOnInvokedFilter(ClientContext context)
		{
			var filters = GetInvokeFilters();
			if (filters == null)
				return;

			foreach (var item in filters)
			{
				try
				{
					item.OnInvoking(context);
				}
				catch (Exception ex)
				{
					LogHelper.Error(ex);
				}
			}
		}

		private void ApplyOnInvokingFilter(ClientContext context)
		{
			var filters = GetInvokeFilters();
			if (filters == null)
				return;

			foreach (var item in filters)
			{
				try
				{
					item.OnInvoked(context);
				}
				catch (Exception ex)
				{
					LogHelper.Error(ex);
				}
			}
		}

		private IClientInvokeFilter[] GetInvokeFilters()
		{
			if (AppHost?.ClientFilters == null || AppHost.ClientFilters.Version == 0)
				return null;

			if (AppHost.ClientFilters.Version == _oldFilterVersion)
				return _invokeFilters;

			var filters = AppHost.ClientFilters
					.Select(it => it as IClientInvokeFilter)
					.Where(it => it != null)
					.ToArray();

			_oldFilterVersion = AppHost.ClientFilters.Version;

			_invokeFilters = filters.Length == 0
				? null
				: filters;
			return filters;
		}

		//class InvokerHolder<TResult>
		//{
		//	public Func<Task<TResult>, ClientContext> Value;
		//}

		//private IInvoker<TContract> _oldInvoker;
		//private Func<Task<TResult>, ClientContext> GetInvokeAsyncFunc<TResult>()  //Task<TResult> InvokeAsync<TResult>(ClientContext request);
		//{
		//	if (AppHost == null)
		//		return Invoker.InvokeAsync<TResult>;

		//	var invoker = Invoker;
		//	if (invoker == _oldInvoker && _oldInvoker == AppHost ?)

		//		_oldInvoker = Invoker;
		//	return null;
		//}

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
