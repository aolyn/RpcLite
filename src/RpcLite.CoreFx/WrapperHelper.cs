using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace RpcLite
{
	//------------------------DynamicInterfaceWrapper类-------------------------------
	public class DynamicInterfaceWrapper
	{
		public static T GetWrapper<T>(object obj) where T : class
		{
			if (obj == null)
				return null;
			return GetWrapperCore<T>(obj) as T;
		}

		private static T GetWrapperCore<T>(object obj) where T : class
		{
			var type = typeof(T);
#if NETCORE
			if (!type.GetTypeInfo().IsInterface)
#else
			if (!type.IsInterface)
#endif
				return obj as T;
			var wrapperType = new WrapperHelper<T>(obj).GetWrapperType();
			if (wrapperType == null)
				return null;
			var result = Activator.CreateInstance(wrapperType, obj);
			return result as T;
		}
	}

	//---------------------InterfaceNotImplementedException错误类----------------------
	public sealed class InterfaceNotImplementedException
		: Exception
	{
		public InterfaceNotImplementedException() { }
	}

	//---------------------WrapperHelper<T>类，这个才是核心----------------------
	public class WrapperHelper<TInterface> where TInterface : class
	{
		#region Consts
		private const TypeAttributes TYPE_ATTRIBUTES = TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Serializable;
		private const FieldAttributes FIELD_ATTRIBUTES = FieldAttributes.Private;
		private const MethodAttributes METHOD_ATTRIBUTES = MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final | MethodAttributes.HideBySig;
		#endregion

		#region Fields
		private object _obj;
		private Type _objType;
		private Type _interfaceType;
		private TypeBuilder _type;
		private FieldBuilder _field;
		#endregion

		#region Ctors
		public WrapperHelper(object obj)
		{
			_obj = obj;
			_objType = obj.GetType();
			_interfaceType = typeof(TInterface);
		}
		#endregion

		#region Private Methods
		private void PrepareType()
		{
			var myAssemblyName = new AssemblyName { Name = RandomName };

#if NETCORE
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.Run);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(RandomName);
#else
			AssemblyBuilder myAssembly = AppDomain.CurrentDomain
				.DefineDynamicAssembly(myAssemblyName, AssemblyBuilderAccess.RunAndSave);
			var moduleBuilder = myAssembly.DefineDynamicModule(RandomName, true);
#endif

			_type = moduleBuilder.DefineType(RandomName, TYPE_ATTRIBUTES, typeof(object), new[] { _interfaceType });
		}
		private void PrepareField()
		{
			_field = _type.DefineField("_source", _interfaceType, FIELD_ATTRIBUTES);
		}
		private void PrepareCtor()
		{
			var myConstructorArgs = new Type[] { _objType };
			var myConstructor = _type.DefineConstructor(
				MethodAttributes.Public, CallingConventions.Standard, myConstructorArgs);
			var myConstructorIL = myConstructor.GetILGenerator();
			myConstructorIL.Emit(OpCodes.Ldarg_0);
			var mySuperConstructor = typeof(object).GetConstructor(new Type[0]);
			myConstructorIL.Emit(OpCodes.Call, mySuperConstructor);
			myConstructorIL.Emit(OpCodes.Ldarg_0);
			myConstructorIL.Emit(OpCodes.Ldarg_1);
			myConstructorIL.Emit(OpCodes.Stfld, _field);
			myConstructorIL.Emit(OpCodes.Ret);
		}
		private void PrepareMethods()
		{
			foreach (var mi in _interfaceType.GetMethods())
				GenMethod(mi);
		}

		private MethodBuilder GenMethod(MethodInfo mi)
		{
			MethodBuilder result;
			Type[] paramTypes;
			ILGenerator ilGen;
			var implMi = FindImplementedMethod(mi);
			paramTypes = GetParameterTypes(mi.GetParameters());
			result = _type.DefineMethod(mi.Name, METHOD_ATTRIBUTES, CallingConventions.Standard, mi.ReturnType, paramTypes);
			ilGen = result.GetILGenerator();
			if (mi.ReturnType != typeof(void))
				ilGen.DeclareLocal(_objType);
			ilGen.Emit(OpCodes.Ldarg_0);
			ilGen.Emit(OpCodes.Ldfld, _field);
			for (var i = 0; i < paramTypes.Length; i++)
			{
				if (i == 0)
					ilGen.Emit(OpCodes.Ldarg_1);
				else if (i == 1)
					ilGen.Emit(OpCodes.Ldarg_2);
				else if (i == 2)
					ilGen.Emit(OpCodes.Ldarg_3);
				else
					ilGen.Emit(OpCodes.Ldarg_S, i + 1);
			}
			ilGen.Emit(OpCodes.Callvirt, implMi);
			if (mi.ReturnType != typeof(void))
			{
				ilGen.Emit(OpCodes.Stloc_0);
				ilGen.Emit(OpCodes.Ldloc_0);
			}
			ilGen.Emit(OpCodes.Ret);
			return result;
		}

		private void PrepareProperties()
		{
			foreach (var pi in _interfaceType.GetProperties())
				GenProperty(pi);
		}

		private void GenProperty(PropertyInfo pi)
		{
			var paramTypes = GetParameterTypes(pi.GetIndexParameters());
			MethodBuilder mb;
			var pb = _type.DefineProperty(
				pi.Name, pi.Attributes, pi.PropertyType, paramTypes);
			if (pi.CanRead)
			{
				mb = GenMethod(pi.GetGetMethod());
				pb.SetGetMethod(mb);
			}
			if (pi.CanWrite)
			{
				mb = GenMethod(pi.GetSetMethod());
				pb.SetSetMethod(mb);
			}
		}

		private void PrepareEvents()
		{
			foreach (var ei in _interfaceType.GetEvents())
				GenEvent(ei);
		}

		private void GenEvent(EventInfo pi)
		{
			MethodBuilder mb;
			var eb = _type.DefineEvent(
				pi.Name, pi.Attributes, pi.EventHandlerType);
			mb = GenMethod(pi.GetAddMethod());
			eb.SetAddOnMethod(mb);
			mb = GenMethod(pi.GetRemoveMethod());
			eb.SetRemoveOnMethod(mb);
		}

		private MethodInfo FindImplementedMethod(MethodInfo mi)
		{
			MethodInfo result;
#if NETCORE
			result = _objType.GetTypeInfo().GetDeclaredMethod(mi.Name);
#else
			result = _objType.GetMethod(mi.Name,
				BindingFlags.Instance | BindingFlags.Public,
				null, CallingConventions.Standard,
				GetParameterTypes(mi.GetParameters()), null);
#endif

			if (result == null || result.ReturnType != mi.ReturnType)
				throw new InterfaceNotImplementedException();
			return result;
		}
		#endregion

		#region Public Members
		public Type GetWrapperType()
		{
			try
			{
				PrepareType();
				PrepareField();
				PrepareCtor();
				PrepareMethods();
				PrepareProperties();
				PrepareEvents();
			}
			catch (InterfaceNotImplementedException)
			{
				return null;
			}
#if NETCORE
			return _type.AsType();
#else
			return _type.CreateType();
#endif
		}
		#endregion

		#region Static Members

		private static string RandomName
		{
			get { return GetRandomName(10); }
		}

		private static string GetRandomName(int count)
		{
			var r = new Random();
			var b = new byte[count];
			r.NextBytes(b);
			var result = Convert.ToBase64String(b);
			result = result.Replace('=', '_').Replace('/', '_').Replace('+', '_');
			return result;
		}

		private static Type[] GetParameterTypes(ParameterInfo[] pis)
		{
			var result = new Type[pis.Length];
			for (var i = 0; i < pis.Length; i++)
				result[i] = pis[i].ParameterType;
			return result;
		}

		#endregion
	}
}
