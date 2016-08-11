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
	public class ActionManager
	{
		//public static ActionManager Instance = new ActionManager();

		private readonly QuickReadConcurrentDictionary<string, RpcAction> _actions = new QuickReadConcurrentDictionary<string, RpcAction>();
		private readonly Type _defaultServiceType;

		/// <summary>
		/// 
		/// </summary>
		public ActionManager() { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		public ActionManager(Type serviceType)
		{
			_defaultServiceType = serviceType;
		}

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
			var actionKey = serviceType.FullName + "." + actionName;
			return _actions.GetOrAdd(actionKey, () => GetActionInternal(serviceType, actionName, actionKey));
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

		private RpcAction GetActionInternal(Type serviceType, string actionName, string actionKey)
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
			Type argumentType;
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
				argumentType = TypeCreator.GetParameterType(method);
				methodFunc = MethodHelper.GetCallMethodFunc(serviceType, argumentType, arguments, method, hasReturn);
			}
			else
			{
				argumentType = TypeCreator.GetParameterType(method);
				methodFunc = MethodHelper.GetCallMethodFunc(serviceType, argumentType, arguments, method, hasReturn);
			}

			var actionInfo = new RpcAction
			{
				Name = actionName,
				ArgumentCount = arguments.Length,
				ArgumentType = argumentType,
				DefaultArgument = GetDefaultValue(argumentType),
				MethodInfo = method,
				HasReturnValue = hasReturn,
				ServiceCreator = TypeCreator.GetCreateInstanceFunc(serviceType),
				//ServiceType = serviceType,
				IsStatic = method.IsStatic,
				IsTask = isTask,
			};

			if (isTask)
			{
				actionInfo.InvokeTask = methodFunc as Func<object, object, object>;
				actionInfo.TaskResultType = method.ReturnType.GetGenericArguments().FirstOrDefault();
			}
			else if (hasReturn)
			{
				actionInfo.Func = methodFunc as Func<object, object, object>;
			}
			else
			{
				actionInfo.Action = methodFunc as Action<object, object>;
			}

			return actionInfo;
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
