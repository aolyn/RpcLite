using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpcLite.Service
{
	internal class ActionHelper
	{
		private static readonly Dictionary<string, ActionInfo> Actions = new Dictionary<string, ActionInfo>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="actionName"></param>
		/// <returns></returns>
		public static ActionInfo GetActionInfo(Type serviceType, string actionName)
		{
			var actionKey = serviceType.FullName + "." + actionName;

			ActionInfo actionInfo;
			if (Actions.TryGetValue(actionKey, out actionInfo))
				return actionInfo;

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

			var isAsync = method.Name.StartsWith("Begin");
			var hasReturn = method.ReturnType.FullName != "System.Void";

			var arguments = method.GetParameters();
			Type argumentType;
			Delegate methodFunc;
			Delegate endMethodFunc = null;
			if (isAsync)
			{
				arguments = arguments
					.Take(arguments.Length - 2)
					.ToArray();
				argumentType = TypeCreator.GetParameterType(method, arguments);
				methodFunc = MethodHelper.GetCallMethodAsyncFunc(serviceType, argumentType, arguments, method, hasReturn);

				var endFuncName = "End" + method.Name.Substring(5);
				var endMethod = MethodHelper.GetActionMethod(serviceType, endFuncName);
				var endMethodHasReturn = endMethod.ReturnType.FullName != "System.Void";
				var endMethodArguments = endMethod.GetParameters();
				hasReturn = endMethodHasReturn;
				endMethodFunc = MethodHelper.GetCallMethodFunc(serviceType, typeof(IAsyncResult), endMethodArguments, endMethod, endMethodHasReturn);
			}
			else
			{
				argumentType = TypeCreator.GetParameterType(method);
				methodFunc = MethodHelper.GetCallMethodFunc(serviceType, argumentType, arguments, method, hasReturn);
			}

			actionInfo = new ActionInfo
			{
				Name = actionKey,
				ArgumentCount = arguments.Length,
				ArgumentType = argumentType,
				MethodInfo = method,
				HasReturnValue = hasReturn,
				ServiceCreator = TypeCreator.GetCreateInstanceFunc(serviceType),
				IsAsync = isAsync,
				IsStatic = method.IsStatic,
			};

			if (hasReturn)
			{
				if (isAsync)
				{
					actionInfo.BeginFunc = methodFunc as Func<object, object, AsyncCallback, object, IAsyncResult>;
					actionInfo.EndFunc = endMethodFunc as Func<object, IAsyncResult, object>;
				}
				else
					actionInfo.Func = methodFunc as Func<object, object, object>;
			}
			else
			{
				if (isAsync)
				{
					actionInfo.EndAction = endMethodFunc as Action<object, IAsyncResult>;
				}
				else
					actionInfo.Action = methodFunc as Action<object, object>;
			}
			Actions.Add(actionKey, actionInfo);
			return actionInfo;
		}

		public static object InvokeAction(ActionInfo actionInfo, object reqArg)
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

		private static object InvokeAcion(ActionInfo actionInfo, object requestArgument, object serviceInstance)
		{
			object resultObj;
			if (actionInfo.HasReturnValue)
			{
				resultObj = actionInfo.Func(serviceInstance, requestArgument);
			}
			else
			{
				actionInfo.Action(serviceInstance, requestArgument);
				resultObj = null;
			}
			return resultObj;
		}

		public static IAsyncResult BeginInvokeAction(ActionInfo actionInfo, ServiceResponse output, object reqArg, AsyncCallback cb, object state)
		{
			var serviceInstance = ServiceFactory.GetService(actionInfo);

			var invokeState = new SeviceInvokeContext
			{
				Service = serviceInstance,
				State = state,
				Action = actionInfo,
				Response = output,
			};

			var ar = actionInfo.BeginFunc(serviceInstance.ServiceObject, reqArg, cb, invokeState);
			return ar;
		}
	}
}
