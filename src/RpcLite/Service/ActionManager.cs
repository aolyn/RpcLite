using System;
using System.Linq;
#if NETCORE
using System.Reflection;
#endif
using System.Threading.Tasks;

namespace RpcLite.Service
{
	/// <summary>
	/// create actions
	/// </summary>
	internal class ActionManager
	{
		//public static ActionManager Instance = new ActionManager();

		private readonly QuickReadConcurrentDictionary<string, RpcAction> _actions = new QuickReadConcurrentDictionary<string, RpcAction>();
		private readonly Type _defaultServiceType;
		private readonly RpcService _service;

		/// <summary>
		/// 
		/// </summary>
		public ActionManager() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="service"></param>
		public ActionManager(RpcService service)
		{
			_service = service;
			_defaultServiceType = service.Type;
		}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="serviceType"></param>
		//public ActionManager(Type serviceType)
		//{
		//	_defaultServiceType = serviceType;
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="actionName"></param>
		/// <returns></returns>
		public RpcAction GetAction(Type serviceType, string actionName)
		{
			if (serviceType == null && _defaultServiceType == null)
			{
				throw new ArgumentOutOfRangeException(nameof(serviceType), "parameter serviceType can't be null when defaultServiceType is null");
			}

			serviceType = serviceType ?? _defaultServiceType;
			var actionKey = /*serviceType.FullName + "." + */actionName;
			return _actions.GetOrAdd(actionKey, k => GetActionInternal(serviceType, actionName));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="actionName"></param>
		/// <returns></returns>
		public RpcAction GetAction(string actionName)
		{
			return GetAction(_defaultServiceType, actionName);
		}

		private RpcAction GetActionInternal(Type serviceType, string actionName)
		{
			var method = MethodHelper.GetActionMethod(serviceType, actionName);

			if (method == null)
			{
				method = MethodHelper.GetActionMethod(serviceType, "Begin" + actionName);
				var endMethod = MethodHelper.GetActionMethod(serviceType, "End" + actionName);
				if (method == null && endMethod == null)
				{
					return null;
				}
			}

			if (method == null)
				return null;

			var hasReturn = method.ReturnType.FullName != "System.Void";

			var arguments = method.GetParameters();
			var argumentType = TypeCreator.GetParameterType(method);

			Delegate methodFunc;
			var isTask = false;
#if NETCORE
			if (method.ReturnType == typeof(Task)
				|| (method.ReturnType.GetTypeInfo().IsGenericType && method.ReturnType.GetTypeInfo().BaseType == typeof(Task)))
#else
			if (method.ReturnType == typeof(Task)
				|| (method.ReturnType.IsGenericType && method.ReturnType.BaseType == typeof(Task)))
#endif
			{
				isTask = true;
				//isAsync = true;
				methodFunc = MethodHelper.GetCallMethodFunc(serviceType, argumentType, arguments, method, hasReturn);
			}
			else
			{
				//argumentType = TypeCreator.GetParameterType(method);
				methodFunc = MethodHelper.GetCallMethodFunc(serviceType, argumentType, arguments, method, hasReturn);
			}

			var action = new RpcAction
			{
				Name = actionName,
				ArgumentCount = arguments.Length,
				ArgumentType = argumentType,
				DefaultArgument = GetDefaultValue(argumentType),
				MethodInfo = method,
				HasReturnValue = hasReturn,
				ServiceCreator = TypeCreator.GetCreateInstanceFunc(serviceType),
				IsStatic = method.IsStatic,
				IsTask = isTask,
				Service = _service,
			};

			if (isTask)
			{
				action.InvokeTask = methodFunc as Func<object, object, object>;
				action.TaskResultType = method.ReturnType.GetGenericArguments().FirstOrDefault();
				action.ResultType = action.TaskResultType;
			}
			else if (hasReturn)
			{
				action.Func = methodFunc as Func<object, object, object>;
				action.ResultType = method.ReturnType;
			}
			else
			{
				action.Action = methodFunc as Action<object, object>;
			}

			return action;
		}

		private static object GetDefaultValue(Type type)
		{
			if (type == null || !type
#if NETCORE
				.GetTypeInfo()
#endif
				.IsValueType) return null;

			return Activator.CreateInstance(type);
		}

	}
}
