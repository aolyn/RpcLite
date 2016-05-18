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
		private static readonly QuickReadConcurrentDictionary<string, RpcAction> Actions = new QuickReadConcurrentDictionary<string, RpcAction>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="actionName"></param>
		/// <returns></returns>
		public static RpcAction GetAction(Type serviceType, string actionName)
		{
			var actionKey = serviceType.FullName + "." + actionName;
			return Actions.GetOrAdd(actionKey, () => GetActionInternal(serviceType, actionName, actionKey));
		}

		private static RpcAction GetActionInternal(Type serviceType, string actionName, string actionKey)
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
				Name = actionKey,
				ArgumentCount = arguments.Length,
				ArgumentType = argumentType,
				MethodInfo = method,
				HasReturnValue = hasReturn,
				ServiceCreator = TypeCreator.GetCreateInstanceFunc(serviceType),
				ServiceType = serviceType,
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

	}
}
