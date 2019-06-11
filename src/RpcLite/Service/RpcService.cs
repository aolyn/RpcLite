using System;
using System.Threading.Tasks;
using RpcLite.Filter;
using RpcLite.Logging;

namespace RpcLite.Service
{
	/// <summary>
	/// Represents service information
	/// </summary>
	public class RpcService
	{
		private readonly ActionManager _actionManager;
		private readonly AppHost _host;
		private readonly MetaInfoBuilder _metaInfoBuilder = new MetaInfoBuilder();
		private VersionedList<IServiceInvokeFilter> _invokeFilters;
		private VersionedList<IProcessFilter> _processFilters;
		internal VersionedList<IActionExecuteFilter> ActionExecuteFilter;
		internal VersionedList<IActionExecutingFilter> ActionExecutingFilter;
		private long _oldVersion;
		private long _processFilterOldVersion;
		Func<ServiceContext, Task> _filterFunc;

		private VersionedList<IServiceFilter> Filters => _host?.ServiceFilters;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="host"></param>
		internal RpcService(Type serviceType, AppHost host)
		{
			Type = serviceType;
			_actionManager = new ActionManager(this, true);
			_host = host;
		}

		#region properties

		/// <summary>
		/// Service request url path
		/// </summary>
		public string Path { get; internal set; }

		/// <summary>
		/// Service's Type
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Name of Service
		/// </summary>
		public string Name { get; internal set; }

		/// <summary>
		/// Use chunked encoding for write response
		/// </summary>
		public bool UseChunkedEncoding { get; set; }

		/// <summary>
		/// Convert to string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name}, {Path}, {Type}";
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal Task ProcessAsync(ServiceContext context)
		{
			LogHelper.Debug("RpcService.BeginProcessRequest");

			if (context.Request.ActionName == "?meta" || string.IsNullOrEmpty(context.Request.ActionName))
			{
				LogHelper.Debug("RpcService.BeginProcessRequest: start ActionHelper.InvokeAction");
				try
				{
					var metaInfo = _metaInfoBuilder.GetMetaInfo(context.Service);
					context.Result = metaInfo;
					context.Request.RequestType = RequestType.MetaData;
				}
				catch (Exception ex)
				{
					context.Exception = ex;
				}
				LogHelper.Debug("RpcService.BeginProcessRequest: end ActionHelper.InvokeAction");

				return TaskHelper.FromResult((object)null);
			}

			LogHelper.Debug("RpcService.BeginProcessRequest: start ActionHelper.GetActionInfo");
			var action = _actionManager.GetAction(context.Request.ActionName);
			LogHelper.Debug("RpcService.BeginProcessRequest: end ActionHelper.GetActionInfo");
			if (action == null)
			{
				LogHelper.Debug("Action Not Found: " + context.Request.ActionName);
				throw new ActionNotFoundException(context.Request.ActionName);
			}

			LogHelper.Debug("RpcService.BeginProcessRequest: got requestObject");

			context.Action = action;

			GroupFilters();
			var filterFunc = GetProcessFilterFunc();

			return filterFunc(context);
		}

		private void GroupFilters()
		{
			if (Filters == null)
			{
				if (_oldVersion == 0)
					return;

				_invokeFilters = null;
				_processFilters = null;
				ActionExecuteFilter = null;
				_oldVersion = 0;
				return;
			}

			if (Filters.Version == _oldVersion)
				return;

			//copy on write instead of lock
			var processFilters = new VersionedList<IProcessFilter>(_processFilters?.Version ?? 0);
			var invokeFilters = new VersionedList<IServiceInvokeFilter>(_invokeFilters?.Version ?? 0);
			var actionExecuteFilter = new VersionedList<IActionExecuteFilter>(ActionExecuteFilter?.Version ?? 0);
			var actionExecutingFilter = new VersionedList<IActionExecutingFilter>(ActionExecutingFilter?.Version ?? 0);

			foreach (var item in Filters)
			{
				switch (item)
				{
					case IProcessFilter filter:
						processFilters.Add(filter);
						break;
					case IServiceInvokeFilter filter:
						invokeFilters.Add(filter);
						break;
					case IActionExecuteFilter filter:
						actionExecuteFilter.Add(filter);
						break;
					case IActionExecutingFilter filter:
						actionExecutingFilter.Add(filter);
						break;
				}
			}

			_processFilters = processFilters;
			_invokeFilters = invokeFilters;
			ActionExecuteFilter = actionExecuteFilter;
			ActionExecutingFilter = actionExecutingFilter;
			_oldVersion = Filters.Version;
		}

		private Func<ServiceContext, Task> GetProcessFilterFunc()
		{
			if (_processFilters == null || _processFilters.Count == 0)
				return ProcessRequestAsync;

			var version = _processFilters.Version;
			if (_processFilterOldVersion == version)
				return _filterFunc;

			//use copy on write instead of lock
			Func<ServiceContext, Task> filterFunc = ProcessRequestAsync;
			var preFilterFunc = filterFunc;
			for (var idxFilter = _processFilters.Count - 1; idxFilter > -1; idxFilter--)
			{
				var thisFilter = _processFilters[idxFilter];

				var nextFilterFunc = preFilterFunc;
				//all currentFilterFunc.GetHashCode is the same
				Func<ServiceContext, Task> currentFilterFunc = sc => thisFilter.ProcessAsync(sc, nextFilterFunc);

				preFilterFunc = currentFilterFunc;
			}
			filterFunc = preFilterFunc;

			_filterFunc = filterFunc;
			_processFilterOldVersion = version;
			return filterFunc;
		}

		private Task ProcessRequestAsync(ServiceContext context)
		{
			ApplyBeforeInvokeFilters(context);

			var task = context.Action.ExecuteAsync(context);
			var waitTask = task.ContinueWith(tsk =>
			{
				ApplyAfterInvokeFilters(context);
			});

			return waitTask;
		}

		private void ApplyAfterInvokeFilters(ServiceContext context)
		{
			if (_invokeFilters == null) return;

			foreach (var item in _invokeFilters)
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

		private void ApplyBeforeInvokeFilters(ServiceContext context)
		{
			if (_invokeFilters == null) return;

			foreach (var item in _invokeFilters)
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
	}
}