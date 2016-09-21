using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RpcLite.Logging;

namespace RpcLite.Service
{
	/// <summary>
	/// Respresnts service infomation
	/// </summary>
	public class RpcService
	{
		private readonly ActionManager _actionManager;
		private VersionedList<IServiceFilter> Filters => _host?.ServiceFilters;
		private VersionedList<IServiceInvokeFilter> _invokeFilters;
		private VersionedList<IProcessFilter> _processFilters;
		internal VersionedList<IActionExecteFilter> ActionExecteFilter;
		private readonly AppHost _host;
		private long _oldVersion;
		private long _processFilterOldVersion;

		Func<ServiceContext, Task> _filterFunc;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="host"></param>
		public RpcService(Type serviceType, AppHost host)
		{
			Type = serviceType;
			_actionManager = new ActionManager(this);
			_host = host;
		}

		#region properties

		/// <summary>
		/// Service request url path
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Service's Type
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Name of Service
		/// </summary>
		public string Name { get; set; }


		///// <summary>
		///// 
		///// </summary>
		//public event Action<ServiceContext> BeforeInvoke;

		///// <summary>
		///// 
		///// </summary>
		//public event Action<ServiceContext> AfterInvoke;

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
		public Task ProcessAsync(ServiceContext context)
		{
			LogHelper.Debug("RpcService.BeginProcessRequest");

			if (context.Request.ActionName == "?meta" || string.IsNullOrEmpty(context.Request.ActionName))
			{
				LogHelper.Debug("RpcService.BeginProcessRequest: start ActionHelper.InvokeAction");
				try
				{
					var metaInfo = GetMetaInfo(context.Service);
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
			var action = _actionManager.GetAction(context.Service.Type, context.Request.ActionName);
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
				ActionExecteFilter = null;
				_oldVersion = 0;
				return;
			}

			if (Filters.Version == _oldVersion)
				return;

			//todo: add lock
			_processFilters = new VersionedList<IProcessFilter>();
			_invokeFilters = new VersionedList<IServiceInvokeFilter>();
			ActionExecteFilter = new VersionedList<IActionExecteFilter>();

			foreach (var item in Filters)
			{
				if (item is IProcessFilter)
				{
					_processFilters.Add(item as IProcessFilter);
				}
				else if (item is IServiceInvokeFilter)
				{
					_invokeFilters.Add(item as IServiceInvokeFilter);
				}
				else if (item is IActionExecteFilter)
				{
					ActionExecteFilter.Add(item as IActionExecteFilter);
				}
			}

			_oldVersion = Filters.Version;
		}

		private Func<ServiceContext, Task> GetProcessFilterFunc()
		{
			if (_processFilters == null || _processFilters.Count == 0)
				return ProcessRequest;

			var version = _processFilters.Version;
			if (_processFilterOldVersion == version)
				return _filterFunc;

			//todo: add lock
			Func<ServiceContext, Task> filterFunc = ProcessRequest;

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

		private Task ProcessRequest(ServiceContext context)
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

		private object GetMetaInfo(RpcService service)
		{
			var type = service.Type;
			var sb = new StringBuilder();
			sb.AppendFormat("Service Name: {0}", type.Name);
			sb.AppendLine();
			sb.Append("Actions:");

			//			var typeInfo =
			//#if NETCORE
			//				type.GetTypeInfo();
			//#else
			//				type;
			//#endif

			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

			foreach (var method in methods)
			{
				if (method.DeclaringType == typeof(object))
					continue;

				sb.AppendLine();
				sb.Append(method.ReturnType.Name);
				sb.Append(" ");
				sb.AppendFormat("{0}(", method.Name);

				var isFirstArgument = true;
				foreach (var arg in method.GetParameters())
				{
					if (isFirstArgument)
						isFirstArgument = false;
					else
						sb.Append(", ");

					sb.Append(arg.ParameterType.Name);
					sb.Append(" ");
					sb.Append(arg.Name);
				}

				sb.AppendFormat(");");
			}

			return sb.ToString();
		}
	}
}