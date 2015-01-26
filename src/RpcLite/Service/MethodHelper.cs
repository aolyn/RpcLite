using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RpcLite.Service
{
	class MethodHelper
	{
		private static readonly QuickReadConcurrentDictionary<string, MethodInfo> ActionMethods = new QuickReadConcurrentDictionary<string, MethodInfo>();
		private static readonly QuickReadConcurrentDictionary<MethodInfo, Delegate> CallMethods = new QuickReadConcurrentDictionary<MethodInfo, Delegate>();
		private static readonly QuickReadConcurrentDictionary<MethodInfo, Delegate> AsyncCallMethods = new QuickReadConcurrentDictionary<MethodInfo, Delegate>();

		public static Delegate GetCallMethodFunc(Type serviceType, Type argumentType, ParameterInfo[] arguments, MethodInfo method, bool hasReturn)
		{
			return CallMethods.GetOrAdd(method, GetCallMethodFuncInternal(serviceType, argumentType, arguments, method, hasReturn));
		}

		private static Delegate GetCallMethodFuncInternal(Type serviceType, Type argumentType, ParameterInfo[] arguments, MethodInfo method, bool hasReturn)
		{
			if (arguments.Length > 0 && argumentType == null)
				throw new ArgumentException("parameterType can not be null when paras.Length > 0");

			var serviceArgument = Expression.Parameter(typeof(object), "service");
			var actionArgument = Expression.Parameter(typeof(object), "argument");

			var convertService = Expression.Convert(serviceArgument, serviceType);
			var convertArgument = argumentType == null ? null : Expression.Convert(actionArgument, argumentType);

			MethodCallExpression call;
			if (arguments.Length > 1)
			{
				if (convertArgument == null)
					throw new ArgumentException("argumentType can't be null when arguments.Length > 1");

				var callArgs = arguments
					.Select(it => Expression.Property(convertArgument, it.Name))
					.ToList();

				call = method.IsStatic
					? Expression.Call(method, callArgs)
					: Expression.Call(convertService, method, callArgs);
			}
			else if (arguments.Length == 1)
			{
				call = method.IsStatic
					? Expression.Call(method, new Expression[] { convertArgument })
					: Expression.Call(convertService, method, new Expression[] { convertArgument });
			}
			else
			{
				call = method.IsStatic
					? Expression.Call(method)
					: Expression.Call(convertService, method);
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

		public static Delegate GetCallMethodAsyncFunc(Type serviceType, Type argumentType, ParameterInfo[] arguments, MethodInfo method, bool hasReturn)
		{
			return AsyncCallMethods.GetOrAdd(method, GetCallMethodAsyncFuncInternal(serviceType, argumentType, arguments, method, hasReturn));
		}

		private static Delegate GetCallMethodAsyncFuncInternal(Type serviceType, Type argumentType, ParameterInfo[] arguments, MethodInfo method, bool hasReturn)
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
				if (convertArgument == null)
					throw new ArgumentException("argumentType can't be null when arguments.Length > 1");

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

		public static MethodInfo GetActionMethod(Type serviceType, string action)
		{
			var methodKey = serviceType.Name + "." + action;
			return ActionMethods.GetOrAdd(methodKey, () =>
			{
				var methods = serviceType.GetMethods();
				var method = methods
					.FirstOrDefault(it => string.CompareOrdinal(it.Name, action) == 0);
				return method;
			});
		}
	}
}
