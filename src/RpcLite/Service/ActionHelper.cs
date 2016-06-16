using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RpcLite.Service
{
	internal class ActionHelper
	{
		private static readonly QuickReadConcurrentDictionary<string, RpcAction> Actions = new QuickReadConcurrentDictionary<string, RpcAction>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="actionName"></param>
		/// <returns></returns>
		public static RpcAction GetActionInfo(Type serviceType, string actionName)
		{
			var actionKey = serviceType.FullName + "." + actionName;
			return Actions.GetOrAdd(actionKey, () => GetActionInfoInternal(serviceType, actionName, actionKey));
		}

		private static RpcAction GetActionInfoInternal(Type serviceType, string actionName, string actionKey)
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

			//var isAsync = method.Name.StartsWith("Begin");
			var hasReturn = method.ReturnType.FullName != "System.Void";

			var arguments = method.GetParameters();
			Type argumentType;
			Delegate methodFunc;
			//Delegate endMethodFunc = null;

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
				//if (isAsync)
				//{
				//	arguments = arguments
				//		.Take(arguments.Length - 2)
				//		.ToArray();
				//	argumentType = TypeCreator.GetParameterType(method, arguments);
				//	methodFunc = MethodHelper.GetCallMethodAsyncFunc(serviceType, argumentType, arguments, method, hasReturn);

				//	var endFuncName = "End" + method.Name.Substring(5);
				//	var endMethod = MethodHelper.GetActionMethod(serviceType, endFuncName);
				//	var endMethodHasReturn = endMethod.ReturnType.FullName != "System.Void";
				//	var endMethodArguments = endMethod.GetParameters();
				//	hasReturn = endMethodHasReturn;
				//	endMethodFunc = MethodHelper.GetCallMethodFunc(serviceType, typeof(IAsyncResult), endMethodArguments, endMethod, endMethodHasReturn);
				//}
				//else
				{
					argumentType = TypeCreator.GetParameterType(method);
					methodFunc = MethodHelper.GetCallMethodFunc(serviceType, argumentType, arguments, method, hasReturn);
				}
			}

			var actionInfo = new RpcAction
			{
				Name = actionKey,
				ArgumentCount = arguments.Length,
				ArgumentType = argumentType,
				MethodInfo = method,
				HasReturnValue = hasReturn,
				ServiceCreator = TypeCreator.GetCreateInstanceFunc(serviceType),
				//IsAsync = isAsync,
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
				//if (isAsync)
				//{
				//	actionInfo.BeginFunc = methodFunc as Func<object, object, AsyncCallback, object, IAsyncResult>;
				//	actionInfo.EndFunc = endMethodFunc as Func<object, IAsyncResult, object>;
				//}
				//else
				actionInfo.Func = methodFunc as Func<object, object, object>;
			}
			else
			{
				//if (isAsync)
				//{
				//	actionInfo.EndAction = endMethodFunc as Action<object, IAsyncResult>;
				//}
				//else
				actionInfo.Action = methodFunc as Action<object, object>;
			}

			return actionInfo;
		}

		public static object InvokeAction(RpcAction actionInfo, object reqArg)
		{
			if (actionInfo.IsStatic)
			{
				return InvokeAcion(actionInfo, reqArg, null);
			}

			using (var serviceInstance = ServiceFactory.GetService(actionInfo))
			{
				return InvokeAcion(actionInfo, reqArg, serviceInstance.ServiceObject);
			}
		}

		private static object InvokeAcion(RpcAction actionInfo, object requestObject, object serviceObject)
		{
			object resultObj;
			if (actionInfo.HasReturnValue)
			{
				resultObj = actionInfo.Func(serviceObject, requestObject);
			}
			else
			{
				actionInfo.Action(serviceObject, requestObject);
				resultObj = null;
			}
			return resultObj;
		}

		//public static IAsyncResult BeginInvokeAction(RpcAction actionInfo, ServiceResponse response, object requestObject, AsyncCallback cb, ServiceContext state)
		//{
		//	object serviceObject = null;
		//	if (!actionInfo.IsStatic)
		//	{
		//		var serviceContainer = ServiceFactory.GetService(actionInfo);
		//		state.ServiceContainer = serviceContainer;
		//		serviceObject = serviceContainer.ServiceObject;
		//	}

		//	var ar = actionInfo.BeginFunc(serviceObject, requestObject, cb, state);
		//	return ar;
		//}

		public static Task InvokeTask(ServiceContext context)
		{
			object serviceObject = null;
			if (!context.Action.IsStatic)
			{
				var serviceContainer = ServiceFactory.GetService(context.Action);
				context.ServiceContainer = serviceContainer;
				serviceObject = serviceContainer.ServiceObject;
			}

			var ar = (Task)context.Action.InvokeTask(serviceObject, context.Argument);
			return ar;
		}
	}
}
