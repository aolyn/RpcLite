using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RpcLite.Service;

namespace RpcLite.Client
{
	internal class ClientActionManager
	{
		private static readonly ConcurrentDictionary<string, ClientActionManager> ActionMangers = new ConcurrentDictionary<string, ClientActionManager>();
		private readonly Type _contractType;
		private readonly QuickReadConcurrentDictionary<string, RpcActionInfo> _actions = new QuickReadConcurrentDictionary<string, RpcActionInfo>();

		private ClientActionManager(string name, Type contractType)
		{
			Name = name;
			_contractType = contractType;
		}

		public static ClientActionManager GetInstance(string name, Type contractType)
		{
			var key = contractType.FullName + "_" + name;
			return ActionMangers.GetOrAdd(key, k =>
			{
				var mgr = new ClientActionManager(name, contractType);
				return mgr;
			});
		}


		public string Name { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="actionName"></param>
		/// <returns></returns>
		public RpcActionInfo GetAction(string actionName)
		{
			var actionKey = /*serviceType.FullName + "." + */ actionName;
			return _actions.GetOrAdd(actionKey, k => GetActionInternal(_contractType, actionName));
		}

		private RpcActionInfo GetActionInternal(Type serviceType, string actionName)
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

			var action = new RpcActionInfo
			{
				Name = actionName,
				ArgumentCount = arguments.Length,
				ArgumentType = argumentType,
				DefaultArgument = GetDefaultValue(argumentType),
				MethodInfo = method,
				HasReturnValue = hasReturn,
				IsTask = isTask,
				ContractType = _contractType,
			};

			if (isTask)
			{
				action.TaskResultType = method.ReturnType.GetGenericArguments().FirstOrDefault();
				action.ResultType = action.TaskResultType;
			}
			else if (hasReturn)
			{
				action.ResultType = method.ReturnType;
			}
			else
			{
				action.ResultType = null;
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