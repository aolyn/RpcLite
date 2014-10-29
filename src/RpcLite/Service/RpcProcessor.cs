using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RpcLite.Formatters;

namespace RpcLite.Service
{
	public static class RpcProcessor
	{
		private static readonly Dictionary<string, MethodInfo> ActionMethods = new Dictionary<string, MethodInfo>();
		private static readonly Dictionary<string, ActionInfo> Actions = new Dictionary<string, ActionInfo>();

		public static object ProcessRequest(ServiceRequest serviceRequest)
		{
			var actionInfo = GetActionInfo(serviceRequest.ServiceType, serviceRequest.ActionName);
			if (actionInfo == null)
				throw new ServiceException("Action Not Found: " + serviceRequest.ActionName);

			object requestObject = null;
			if (actionInfo.ArgumentCount > 0)
				requestObject = GetRequestObject(serviceRequest.InputStream, serviceRequest.Formatter, actionInfo.ArgumentType);

			var result = InvokeAction(actionInfo, requestObject);
			if (actionInfo.HasReturnValue)
				return result;

			return NullResponse.Value;
		}

		public static IAsyncResult BeginProcessRequest(ServiceRequest serviceRequest, ServiceResponse outputStream, AsyncCallback cb, object state)
		{
			var actionInfo = GetActionInfo(serviceRequest.ServiceType, serviceRequest.ActionName);
			if (actionInfo == null)
				throw new ServiceException("Action Not Found: " + serviceRequest.ActionName);

			object requestObject = null;
			if (actionInfo.ArgumentCount > 0)
				requestObject = GetRequestObject(serviceRequest.InputStream, serviceRequest.Formatter, actionInfo.ArgumentType);

			var result = BeginInvokeAction(actionInfo, outputStream, requestObject, cb, state);

			return result;
		}

		private static object GetRequestObject(Stream stream, IFormatter formatter, Type type)
		{
			try
			{
				return formatter.Deserilize(stream, type);
			}
			catch (Exception)
			{
				throw new RequestException("Parse request content to object error.");
			}
		}

		private static object InvokeAction(ActionInfo actionInfo, object reqArg)
		{
			using (var serviceInstance = ServiceFactory.GetService(actionInfo))
			{
				object resultObj;
				if (actionInfo.HasReturnValue)
				{
					resultObj = actionInfo.Func(serviceInstance.ServiceObject, reqArg);
				}
				else
				{
					actionInfo.Action(serviceInstance.ServiceObject, reqArg);
					resultObj = null;
				}
				return resultObj;
			}
		}

		private static IAsyncResult BeginInvokeAction(ActionInfo actionInfo, ServiceResponse output, object reqArg, AsyncCallback cb, object state)
		{
			var serviceInstance = ServiceFactory.GetService(actionInfo);

			var invokeState = new SeviceInvokeContext
			{
				Service = serviceInstance,
				State = state,
				Action = actionInfo,
				Output = output,
			};

			var ar = actionInfo.BeginFunc(serviceInstance.ServiceObject, reqArg, cb, invokeState);
			return ar;
		}

		public static ActionInfo GetActionInfo(Type serviceType, string actionName)
		{
			var actionKey = serviceType.FullName + "." + actionName;

			ActionInfo actionInfo;
			if (Actions.TryGetValue(actionKey, out actionInfo))
				return actionInfo;

			var method = GetActionMethod(serviceType, actionName);

			if (method == null)
				return null;

			var isAsync = actionName.StartsWith("Begin");
			var hasReturn = method.ReturnType.FullName != "System.Void";

			var arguments = method.GetParameters();
			Type argumentType = null;
			Delegate methodFunc = null;
			Delegate endMethodFunc = null;
			if (isAsync)
			{
				arguments = arguments
					.Take(arguments.Length - 2)
					.ToArray();
				argumentType = TypeCreator.GetParameterType(method, arguments);
				methodFunc = GetCallMethodAsyncFunc(serviceType, argumentType, arguments, method, hasReturn);

				var endFuncName = "End" + method.Name.Substring(5);
				var endMethod = GetActionMethod(serviceType, endFuncName);
				var endMethodHasReturn = endMethod.ReturnType.FullName != "System.Void";
				var endMethodArguments = endMethod.GetParameters();
				hasReturn = endMethodHasReturn;
				endMethodFunc = GetEndMethodAsyncFunc(serviceType, typeof(IAsyncResult), endMethodArguments, endMethod, endMethodHasReturn);
			}
			else
			{
				argumentType = TypeCreator.GetParameterType(method);
				methodFunc = GetCallMethodFunc(serviceType, argumentType, arguments, method, hasReturn);
			}

			actionInfo = new ActionInfo
			{
				Name = actionKey,
				ArgumentCount = arguments.Length,
				ArgumentType = argumentType,
				MethodInfo = method,
				HasReturnValue = hasReturn,
				ServiceCreator = TypeCreator.GetCreateInstanceFunc(serviceType)
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
					actionInfo.EndAction = endMethodFunc as Action<IAsyncResult>;
				}
				else
					actionInfo.Action = methodFunc as Action<object, object>;
			}
			Actions.Add(actionKey, actionInfo);
			return actionInfo;
		}

		private static Delegate GetCallMethodFunc(Type serviceType, Type argumentType, ParameterInfo[] arguments, MethodInfo method, bool hasReturn)
		{
			if (arguments.Length > 0 && argumentType == null)
				throw new ArgumentException("parameterType can not be null when paras.Length > 0");

			var serviceArgument = Expression.Parameter(typeof(object), "service");
			var actionArgument = Expression.Parameter(typeof(object), "arg");

			var convertService = Expression.Convert(serviceArgument, serviceType);
			var convertArgument = argumentType == null ? null : Expression.Convert(actionArgument, argumentType);

			MethodCallExpression call;
			if (arguments.Length > 1)
			{
				var callArgs = arguments
					.Select(it => Expression.Property(convertArgument, it.Name))
					.ToList();

				call = Expression.Call(convertService, method, callArgs);
			}
			else if (arguments.Length == 1)
			{
				call = Expression.Call(convertService, method, new Expression[] { convertArgument });
			}
			else
			{
				call = Expression.Call(convertService, method);
			}

			var methodArgs = new[] { serviceArgument, actionArgument };

			LambdaExpression methodLam;
			if (hasReturn)
			{
				var convertCall = Expression.Convert(call, typeof(object));
				methodLam = Expression.Lambda(convertCall, methodArgs);
			}
			else
			{
				methodLam = Expression.Lambda(call, methodArgs);
			}

			var methodFunc = methodLam.Compile();
			return methodFunc;
		}

		private static Delegate GetCallMethodAsyncFunc(Type serviceType, Type argumentType, ParameterInfo[] arguments, MethodInfo method, bool hasReturn)
		{
			if (arguments.Length > 0 && argumentType == null)
				throw new ArgumentException("parameterType can not be null when paras.Length > 0");

			var serviceArgument = Expression.Parameter(typeof(object), "service");
			var actionArgument = Expression.Parameter(typeof(object), "argument");
			var asyncCallbackArgument = Expression.Parameter(typeof(AsyncCallback), "callback");
			var stateArgument = Expression.Parameter(typeof(object), "state");

			var convertService = Expression.Convert(serviceArgument, serviceType);
			var convertArgument = argumentType == null ? null : Expression.Convert(actionArgument, argumentType);

			MethodCallExpression call;
			if (arguments.Length > 1)
			{
				var callArgs = arguments
					.Select(it => (Expression)Expression.Property(convertArgument, it.Name))
					.ToList();

				callArgs.Add(asyncCallbackArgument);
				callArgs.Add(stateArgument);

				call = Expression.Call(convertService, method, callArgs);
			}
			else if (arguments.Length == 1)
			{
				call = Expression.Call(convertService, method, new Expression[] { convertArgument, asyncCallbackArgument, stateArgument });
			}
			else
			{
				call = Expression.Call(convertService, method, new Expression[] { asyncCallbackArgument, stateArgument });
			}

			var methodArgs = new[] { serviceArgument, actionArgument, asyncCallbackArgument, stateArgument };

			LambdaExpression methodLam;
			if (hasReturn)
			{
				var convertCall = Expression.Convert(call, typeof(IAsyncResult));
				methodLam = Expression.Lambda(convertCall, methodArgs);
			}
			else
			{
				methodLam = Expression.Lambda(call, methodArgs);
			}

			var methodFunc = methodLam.Compile();
			return methodFunc;
		}

		private static Delegate GetEndMethodAsyncFunc(Type serviceType, Type argumentType, ParameterInfo[] arguments, MethodInfo method, bool hasReturn)
		{
			if (arguments.Length > 0 && argumentType == null)
				throw new ArgumentException("parameterType can not be null when paras.Length > 0");

			var serviceArgument = Expression.Parameter(typeof(object), "service");
			var actionArgument = Expression.Parameter(typeof(object), "argument");
			//var asyncCallbackArgument = Expression.Parameter(typeof(AsyncCallback), "callback");
			//var stateArgument = Expression.Parameter(typeof(object), "state");
			//var stateArgument = Expression.Parameter(typeof(object), "state");

			var convertService = Expression.Convert(serviceArgument, serviceType);
			var convertArgument = argumentType == null ? null : Expression.Convert(actionArgument, argumentType);

			MethodCallExpression call;
			if (arguments.Length > 1)
			{
				var callArgs = arguments
					.Select(it => (Expression)Expression.Property(convertArgument, it.Name))
					.ToList();

				//callArgs.Add(asyncCallbackArgument);
				//callArgs.Add(stateArgument);

				call = Expression.Call(convertService, method, callArgs);
			}
			else if (arguments.Length == 1)
			{
				call = Expression.Call(convertService, method, new Expression[] { convertArgument });
			}
			else
			{
				call = Expression.Call(convertService, method, new Expression[] { });
			}

			var methodArgs = new[] { serviceArgument, actionArgument };

			LambdaExpression methodLam;
			if (hasReturn)
			{
				var convertCall = Expression.Convert(call, typeof(object));
				methodLam = Expression.Lambda(convertCall, methodArgs);
			}
			else
			{
				methodLam = Expression.Lambda(call, methodArgs);
			}

			var methodFunc = methodLam.Compile();
			return methodFunc;
		}

		private static MethodInfo GetActionMethod(Type serviceType, string action)
		{
			var methodKey = serviceType.Name + "." + action;
			MethodInfo method;
			if (!ActionMethods.TryGetValue(methodKey, out method))
			{
				var methods = serviceType.GetMethods();
				method = methods
					.FirstOrDefault(it => string.CompareOrdinal(it.Name, action) == 0);
				if (method != null)
				{
					ActionMethods.Add(methodKey, method);
				}
			}
			return method;
		}
	}
}