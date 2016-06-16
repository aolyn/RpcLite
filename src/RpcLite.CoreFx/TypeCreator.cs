﻿//#define SAVE_DLL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace RpcLite
{
	/// <summary>
	/// 
	/// </summary>
	public class TypeCreator
	{
		private static ModuleBuilder _moduleBuilder;
		private static AssemblyBuilder _assemblyBuilder;

		static TypeCreator()
		{
			var result = InitializeModuleBuilder();
			_assemblyBuilder = result.Item1;
			_moduleBuilder = result.Item2;
		}

		private static Tuple<AssemblyBuilder, ModuleBuilder> InitializeModuleBuilder()
		{
			var assembliyName = "TypeCreateAssembly";
#if SAVE_DLL
			assembliyName += Guid.NewGuid().ToString().Replace("-", "");
#endif
			var asmName = new AssemblyName { Name = assembliyName };
			var asmFileName = asmName.Name + ".dll";

#if NETCORE
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(asmFileName);
#else
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(asmFileName, asmFileName, false);
#endif

			return new Tuple<AssemblyBuilder, ModuleBuilder>(assemblyBuilder, moduleBuilder);
		}

		/// <summary>
		/// 
		/// </summary>
		public static Type CreateType(string name, List<PropertyItemInfo> propeties)
		{
#if SAVE_DLL
			var result = InitializeModuleBuilder();
			var assemblyBuilder = result.Item1;
			var modBuilder = result.Item2;
#else
			var assemblyBuilder = _assemblyBuilder;
			var modBuilder = _moduleBuilder;
#endif

			if (propeties == null)
				return null;

			const TypeAttributes typeAttribute = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Serializable;

			var typeBuilder = modBuilder.DefineType(name, typeAttribute);

			var fieldBuilders = new List<FieldBuilder>();
			foreach (var propety in propeties)
			{
				fieldBuilders.Add(AddProperty(propety.Name, propety.Type, typeBuilder));
			}

			typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
			var paraTypes = propeties
				.Select(it => it.Type)
				.ToArray();

			#region create SetValues method

			var setValuesMethod = typeBuilder.DefineMethod("SetValues", MethodAttributes.Public, typeof(void), paraTypes);
			var setValuesIL = setValuesMethod.GetILGenerator();
			setValuesIL.Emit(OpCodes.Nop);

			if (paraTypes.Length > 0)
			{
				setValuesIL.Emit(OpCodes.Ldarg_0);
				setValuesIL.Emit(OpCodes.Ldarg_1);
				setValuesIL.Emit(OpCodes.Stfld, fieldBuilders[0]);
			}

			if (paraTypes.Length > 1)
			{
				setValuesIL.Emit(OpCodes.Ldarg_0);
				setValuesIL.Emit(OpCodes.Ldarg_2);
				setValuesIL.Emit(OpCodes.Stfld, fieldBuilders[1]);
			}

			if (paraTypes.Length > 2)
			{
				setValuesIL.Emit(OpCodes.Ldarg_0);
				setValuesIL.Emit(OpCodes.Ldarg_3);
				setValuesIL.Emit(OpCodes.Stfld, fieldBuilders[2]);
			}

			if (paraTypes.Length > 3)
			{
				for (var i = 3; i < paraTypes.Length; i++)
				{
					setValuesIL.Emit(OpCodes.Ldarg_0);
					setValuesIL.Emit(OpCodes.Ldarg_S, i + 1);
					setValuesIL.Emit(OpCodes.Stfld, fieldBuilders[i]);
				}
			}

			setValuesIL.Emit(OpCodes.Ret);
			#endregion

#if NETCORE
			var type = typeBuilder.CreateTypeInfo().AsType();
#else
			var type = typeBuilder.CreateType();
#endif

#if SAVE_DLL
			//var dllFilePath = _assemblyBuilder.GetName().Name + DateTime.Now.ToString("yyyy-MM-dd HHmmss-ffff") + ".dll";
			var dllFilePath = "AutoGenerated - " + DateTime.Now.ToString("yyyy-MM-dd HHmmss-ffff  ") + type.Name + ".dll";
			assemblyBuilder.Save(dllFilePath);//for test
#endif

			//AssemblyBuilder.Save(@"aa.dll");
			return type;
		}

		/// <summary>
		/// 
		/// </summary>
		private static FieldBuilder AddProperty(string name, Type type, TypeBuilder typeBuilder)
		{
			var field = typeBuilder.DefineField("__@" + name, type, FieldAttributes.Private);
			var property = typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, type, null);

			//create the FirstName Getter
			var getter = typeBuilder.DefineMethod("get_" + name,
				MethodAttributes.Public | MethodAttributes.SpecialName |
				MethodAttributes.HideBySig, type, Type.EmptyTypes);
			var getterIl = getter.GetILGenerator();
			getterIl.Emit(OpCodes.Ldarg_0);
			getterIl.Emit(OpCodes.Ldfld, field);
			getterIl.Emit(OpCodes.Ret);

			//create the FirstName Setter
			var setter = typeBuilder.DefineMethod("set_" + name,
				MethodAttributes.Public | MethodAttributes.SpecialName |
				MethodAttributes.HideBySig, null, new[] { type });
			var setterIl = setter.GetILGenerator();
			setterIl.Emit(OpCodes.Ldarg_0);
			setterIl.Emit(OpCodes.Ldarg_1);
			setterIl.Emit(OpCodes.Stfld, field);
			setterIl.Emit(OpCodes.Ret);

			//assign getter and setter
			property.SetGetMethod(getter);
			property.SetSetMethod(setter);

			return field;
		}

		private readonly static Dictionary<Type, Type> InterfaceImplementTypes = new Dictionary<Type, Type>();
		/// <summary>
		/// thread unsafe
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="parentType"></param>
		/// <returns></returns>
		public static Type WrapInterface<T>(Type parentType)
		{
			var interfaceType = typeof(T);
			Type implementType;
			if (InterfaceImplementTypes.TryGetValue(interfaceType, out implementType))
			{
				return implementType;
			}

#if SAVE_DLL
			var result = InitializeModuleBuilder();
			var assemblyBuilder = result.Item1;
			var modBuilder = result.Item2;
#else
			var assemblyBuilder = _assemblyBuilder;
			var modBuilder = _moduleBuilder;
#endif

			//新类型的属性：要创建的是Class，而非Interface，Abstract Class等，而且是Public的
			const TypeAttributes typeAttribute = TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Serializable;

			var name = interfaceType.FullName.Replace(".", "_").Replace("+", "__") + "_ImplementType";
			//声明要创建的新类型的父类型
			var typeBuilder = modBuilder.DefineType(name, typeAttribute, parentType, new[] { interfaceType });

			var methods = interfaceType.GetMethods();
			foreach (var method in methods)
			{
				BuildMethod(typeBuilder, method);
			}

			//真正创建，并返回
#if NETCORE
			implementType = typeBuilder.CreateTypeInfo().AsType();
#else
			implementType = typeBuilder.CreateType();
#endif


#if SAVE_DLL
			//var dllFilePath = _assemblyBuilder.GetName().Name + DateTime.Now.ToString("yyyy-MM-dd HHmmss-ffff") + ".dll";
			var dllFilePath = "AutoGenerated - " + DateTime.Now.ToString("yyyy-MM-dd HHmmss-ffff  ") + implementType.Name + ".dll";
			assemblyBuilder.Save(dllFilePath);//for test
#endif
			InterfaceImplementTypes.Add(interfaceType, implementType);
			//_assemblyBuilder.Save(@"rpc2.dll");
			return implementType;
		}

		private static void BuildMethod(TypeBuilder typeBuilder, MethodInfo method)
		{
			if (typeBuilder == null) throw new ArgumentNullException(nameof(typeBuilder));

			if (typeBuilder.BaseType == null) throw new ArgumentException("typeBuilder.BaseType is null");

			const MethodAttributes methodAttributes = MethodAttributes.Public
				| MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig;

			var paramTypes = method.GetParameters().Select(it => it.ParameterType).ToArray();
			var methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes, method.ReturnType, paramTypes);
			var methodIL = methodBuilder.GetILGenerator();
			methodIL.Emit(OpCodes.Nop);

			var methodGetTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");

			Type taskParam = null;
			string callMethodName;
			if (method.ReturnType == typeof(Task))
			{
				callMethodName = "GetResponseAsync";
			}
#if NETCORE
			else if (method.ReturnType.GetTypeInfo().IsGenericType && method.ReturnType.GetTypeInfo().BaseType == typeof(Task))
#else
			else if (method.ReturnType.IsGenericType && method.ReturnType.BaseType == typeof(Task))
#endif
			{
				callMethodName = "GetResponseAsync";
				taskParam = method.ReturnType.GetGenericArguments()[0];
			}
			else
			{
				callMethodName = "GetResponse";
			}

			// ReSharper disable once PossibleNullReferenceException
			var getResponse = typeBuilder.BaseType.GetMethod(callMethodName, BindingFlags.Instance | BindingFlags.NonPublic);
			if (taskParam != null)
				getResponse = getResponse.MakeGenericMethod(taskParam);
			var returnType = method.ReturnType;
			var hasReturn = (returnType != typeof(void));
			var paramCount = paramTypes.Length;

			var paramType = GetParameterType(method);
			if (paramCount > 1)
			{
				var ctor = paramType.GetConstructor(Type.EmptyTypes);
				if (ctor == null)
					throw new Exception(paramType.Name + " has no constructor without parameters");

				var setValues = paramType.GetMethod("SetValues", paramTypes);

				methodIL.DeclareLocal(paramType);
				methodIL.DeclareLocal(returnType);


				//set values
				methodIL.Emit(OpCodes.Nop);
				methodIL.Emit(OpCodes.Newobj, ctor);
				methodIL.Emit(OpCodes.Stloc_0);
				methodIL.Emit(OpCodes.Ldloc_0);
				if (paramCount > 0) methodIL.Emit(OpCodes.Ldarg_1);
				if (paramCount > 1) methodIL.Emit(OpCodes.Ldarg_2);
				if (paramCount > 2) methodIL.Emit(OpCodes.Ldarg_3);
				for (byte paramIndex = 4; paramIndex <= paramCount; paramIndex++)
				{
					methodIL.Emit(OpCodes.Ldarg_S, paramIndex);
				}
				methodIL.EmitCall(OpCodes.Callvirt, setValues, paramTypes);
				methodIL.Emit(OpCodes.Nop);


				//GetResponse
				methodIL.Emit(OpCodes.Ldarg_0);
				methodIL.Emit(OpCodes.Ldstr, method.Name);
				methodIL.Emit(OpCodes.Ldloc_0);
			}
			else
			{
				if (hasReturn)
					methodIL.DeclareLocal(returnType);

				methodIL.Emit(OpCodes.Nop);
				methodIL.Emit(OpCodes.Ldarg_0);
				methodIL.Emit(OpCodes.Ldstr, method.Name);

				if (paramCount == 1)
				{
					methodIL.Emit(OpCodes.Ldarg_1);
					//set request object
#if NETCORE
					if (paramType.GetTypeInfo().IsValueType)
#else
					if (paramType.IsValueType)
#endif
					{
						methodIL.Emit(OpCodes.Box, paramType);
					}
				}
				else
				{
					methodIL.Emit(OpCodes.Ldnull);
				}
			}


			//set parameter returnType of "GetResponse(string action, object request, Type returnType)"
			if (hasReturn)
			{
				methodIL.Emit(OpCodes.Ldtoken, returnType);
				methodIL.EmitCall(OpCodes.Call, methodGetTypeFromHandle, new[] { typeof(RuntimeTypeHandle) });
			}
			else
			{
				methodIL.Emit(OpCodes.Ldnull);
			}
			methodIL.EmitCall(OpCodes.Call, getResponse, null);

			//return response
			if (hasReturn)
			{
#if NETCORE
				if (returnType.GetTypeInfo().IsValueType)
#else
				if (returnType.IsValueType)
#endif
					methodIL.Emit(OpCodes.Unbox_Any, returnType);
				else
					methodIL.Emit(OpCodes.Castclass, returnType);
			}
			else
			{
				methodIL.Emit(OpCodes.Pop);
			}
			methodIL.Emit(OpCodes.Ret);
		}

		private static readonly Dictionary<string, Type> ActionParameterTypes = new Dictionary<string, Type>();
		/// <summary>
		/// 
		/// </summary>
		public static Type GetParameterType(MethodBase method)
		{
			var paras = method.GetParameters();
			return GetParameterType(method, paras);
		}

		/// <summary>
		/// 
		/// </summary>
		public static Type GetParameterType(MethodBase method, ParameterInfo[] paras)
		{
			var declareType = method.DeclaringType;
			if (declareType == null) return null;

			var methodName = method.Name;
			var newTypeName = $"{declareType.FullName}RequestTypes.{methodName}RequestType";
			newTypeName = null;
			return GetParameterType(newTypeName, paras, declareType, methodName);
		}

		/// <summary>
		/// 
		/// </summary>
		public static Type GetParameterType(string newTypeName, ParameterInfo[] paras, Type declareType, string methodName)
		{
			Type parameterType = null;
			//prepare call parameter
			if (paras.Length > 1)
			{
				var paraTypeName = newTypeName
					?? string.Format("{0}_{1}_{2}_ParameterType", declareType.FullName.Replace(".", "_").Replace("+", "__"), methodName, paras.Length);
				if (!ActionParameterTypes.TryGetValue(paraTypeName, out parameterType))
				{
					var properties = paras
						.Select(it => new PropertyItemInfo
						{
							Name = it.Name,
							Type = it.ParameterType,
						})
						.ToList();

					parameterType = CreateType(paraTypeName, properties);
					ActionParameterTypes.Add(paraTypeName, parameterType);
				}
			}
			else if (paras.Length > 0)
			{
				parameterType = paras[0].ParameterType;
			}
			return parameterType;
		}

		private static readonly Dictionary<Type, Func<object>> CreateInstanceFuncs = new Dictionary<Type, Func<object>>();
		/// <summary>
		/// get Create Instance Function as: () = > new serviceType()
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		/// <summary>
		/// 
		/// </summary>
		public static Func<object> GetCreateInstanceFunc(Type serviceType)
		{
			if (serviceType == null)
				throw new ArgumentNullException(nameof(serviceType));

			Func<object> func;
			if (CreateInstanceFuncs.TryGetValue(serviceType, out func))
				return func;

			//#if NETCORE
			//			var ctor1 = serviceType.GetConstructors();
			//#else
			//#endif

			var ctor = serviceType.GetConstructor(Type.EmptyTypes);
			if (ctor == null)
				return null;

			var newServiceBody = Expression.New(ctor);
			var expression = Expression.Lambda(newServiceBody);
			var del = expression.Compile() as Func<object>;

			CreateInstanceFuncs.Add(serviceType, del);

			return del;
		}

		/// <summary>
		/// Get type from assembly
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		public static Type GetTypeFromName(string typeName, string assemblyName)
		{
			Assembly assembly;

#if NETCORE
			var asmName = new AssemblyName(assemblyName);
			assembly = Assembly.Load(asmName);
#else
			if (!string.IsNullOrWhiteSpace(assemblyName))
			{
				var asms = AppDomain.CurrentDomain.GetAssemblies();
				assembly = asms.FirstOrDefault(it => it.FullName.StartsWith(assemblyName + ",", StringComparison.OrdinalIgnoreCase))
					?? Assembly.Load(assemblyName);
			}
			else
			{
				assembly = Assembly.GetEntryAssembly();
			}
#endif

			var typeInfo = assembly.GetType(typeName);
			return typeInfo;
		}

	}
}
