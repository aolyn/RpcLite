using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace RpcLite.Utility
{
	/// <summary>
	/// 
	/// </summary>
	public static class ReflectHelper
	{
		//private ConcurrentDictionary<FieldInfo, Func<TTarget, TValue>>
		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="fieldName"></param>
		/// <typeparam name="TTarget"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <returns></returns>
		public static Func<TTarget, TValue> GetFieldGetterFunc<TTarget, TValue>(Type type, string fieldName)
		{
			var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
				return null;

			var dm = new DynamicMethod("Get" + field.Name, typeof(TValue), new[] { typeof(TTarget) }, typeof(TTarget));
			var il = dm.GetILGenerator();
			// Load the instance of the object (argument 0) onto the stack
			il.Emit(OpCodes.Ldarg_0);
			// Load the value of the object's field (fi) onto the stack
			il.Emit(OpCodes.Ldfld, field);
			// return the value on the top of the stack
			il.Emit(OpCodes.Ret);

			var getter = (Func<TTarget, TValue>)dm.CreateDelegate(typeof(Func<TTarget, TValue>));
			return getter;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		public class PropertyGetFieldSetValueSource<TValue>
		{
			private FieldInfo _field;
			private PropertyInfo _property;

			/// <summary>
			/// 
			/// </summary>
			/// <param name="property"></param>
			/// <param name="field"></param>
			public PropertyGetFieldSetValueSource(PropertyInfo property, FieldInfo field)
			{
				_property = property;
				_field = field;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="target"></param>
			/// <returns></returns>
			public TValue GetValue(object target)
			{
#if NETCORE
				return (TValue)_property.GetValue(target);
#else
				return (TValue)_property.GetValue(target, new object[0]);
#endif

			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="target"></param>
			/// <param name="value"></param>
			public void SetValue(object target, TValue value)
			{
				_field.SetValue(target, value);
			}
		}

	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TTarget"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class PropertyReflector<TTarget, TValue>
	{
		private static readonly ConcurrentDictionary<FieldInfo, Func<TTarget, TValue>> FieldGetterCache = new ConcurrentDictionary<FieldInfo, Func<TTarget, TValue>>();
		private static readonly ConcurrentDictionary<FieldInfo, Action<TTarget, TValue>> FieldSetterCache = new ConcurrentDictionary<FieldInfo, Action<TTarget, TValue>>();

		private static readonly ConcurrentDictionary<PropertyInfo, Func<TTarget, TValue>> PropertyGetterCache = new ConcurrentDictionary<PropertyInfo, Func<TTarget, TValue>>();
		private static readonly ConcurrentDictionary<PropertyInfo, Action<TTarget, TValue>> PropertySetterCache = new ConcurrentDictionary<PropertyInfo, Action<TTarget, TValue>>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static Func<TTarget, TValue> GetFieldGetterFunc(string fieldName)
		{
			var type = typeof(TTarget);
			var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
				return null;

			var getterFunc = FieldGetterCache.GetOrAdd(field, f =>
			{
				var getterDelegate = PropertyReflector.MakeFieldGetter(field);

				var getter = (Func<TTarget, TValue>)getterDelegate;
				return getter;
			});

			return getterFunc;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static Action<TTarget, TValue> GetFieldSetterFunc(string fieldName)
		{
			var type = typeof(TTarget);
			var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
				return null;

			var getterFunc = FieldSetterCache.GetOrAdd(field, f =>
			{
				var getterDelegate = PropertyReflector.MakeFieldSetter(field);

				var getter = (Action<TTarget, TValue>)getterDelegate;
				return getter;
			});
			return getterFunc;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static Func<TTarget, TValue> GetPropertyGetterFunc(string fieldName)
		{
			var type = typeof(TTarget);
			var field = type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
				return null;

			var getterFunc = PropertyGetterCache.GetOrAdd(field, f =>
			{
				var getterDelegate = PropertyReflector.MakePropertyGetter(field);

				var getter = (Func<TTarget, TValue>)getterDelegate;
				return getter;
			});
			return getterFunc;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldName"></param>
		/// <returns></returns>
		public static Action<TTarget, TValue> GetPropertySetterFunc(string fieldName)
		{
			var type = typeof(TTarget);
			var field = type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
				return null;

			var getterFunc = PropertySetterCache.GetOrAdd(field, f =>
			{
				Delegate getterDelegate = PropertyReflector.MakePropertySetter(field);

				var getter = (Action<TTarget, TValue>)getterDelegate;
				return getter;
			});
			return getterFunc;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public static class PropertyReflector
	{
		private static readonly ConcurrentDictionary<MemberInfo, Delegate> GetterCache = new ConcurrentDictionary<MemberInfo, Delegate>();
		private static readonly ConcurrentDictionary<MemberInfo, Delegate> SetterCache = new ConcurrentDictionary<MemberInfo, Delegate>();

		private static readonly ConcurrentDictionary<MemberInfo, Func<object, object>> ObjectGetterCache = new ConcurrentDictionary<MemberInfo, Func<object, object>>();
		private static readonly ConcurrentDictionary<MemberInfo, Action<object, object>> ObjectSetterCache = new ConcurrentDictionary<MemberInfo, Action<object, object>>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static Delegate MakeFieldGetter(FieldInfo field)
		{
			var result = GetterCache.GetOrAdd(field, m =>
			{
				var valueType = field.FieldType;
				var targetType = field.DeclaringType;

				var dm = new DynamicMethod("Get" + field.Name, valueType, new[] { targetType }, targetType);
				var il = dm.GetILGenerator();
				// Load the instance of the object (argument 0) onto the stack
				il.Emit(OpCodes.Ldarg_0);
				// Load the value of the object's field (fi) onto the stack
				il.Emit(OpCodes.Ldfld, field);
				// return the value on the top of the stack
				il.Emit(OpCodes.Ret);

				var delegateType = typeof(Func<,>).MakeGenericType(targetType, valueType);
				var getterDelegate = dm.CreateDelegate(delegateType);
				return getterDelegate;
			});
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static Delegate MakeFieldSetter(FieldInfo field)
		{
			var result = SetterCache.GetOrAdd(field, m =>
			{
				var valueType = field.FieldType;
				var targetType = field.DeclaringType;

				var dm = new DynamicMethod("Set" + field.Name, null, new[] { targetType, valueType }, targetType);

				var il = dm.GetILGenerator();
				// Load the instance of the object (argument 0) onto the stack
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldarg_1);
				// Load the value of the object's field (fi) onto the stack
				il.Emit(OpCodes.Stfld, field);
				// return the value on the top of the stack
				il.Emit(OpCodes.Ret);
				var delegateType = typeof(Action<,>).MakeGenericType(targetType, valueType);
				var getterDelegate = dm.CreateDelegate(delegateType);
				return getterDelegate;
			});
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static Delegate MakePropertyGetter(PropertyInfo field)
		{
			var result = GetterCache.GetOrAdd(field, m =>
			{
				var valueType = field.PropertyType;
				var targetType = field.DeclaringType;

				var dm = new DynamicMethod("Get" + field.Name, valueType, new[] { targetType }, targetType);
				var il = dm.GetILGenerator();
				// Load the instance of the object (argument 0) onto the stack
				il.Emit(OpCodes.Ldarg_0);
				// Load the value of the object's field (fi) onto the stack
#if NETCORE
				il.Emit(OpCodes.Callvirt, field.GetMethod);
#else
				il.Emit(OpCodes.Callvirt, field.GetGetMethod());
#endif
				// return the value on the top of the stack
				il.Emit(OpCodes.Ret);
				var delegateType = typeof(Func<,>).MakeGenericType(targetType, valueType);
				var getterDelegate = dm.CreateDelegate(delegateType);
				return getterDelegate;
			});
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static Delegate MakePropertySetter(PropertyInfo field)
		{
			var result = SetterCache.GetOrAdd(field, m =>
			{
				var valueType = field.PropertyType;
				var targetType = field.DeclaringType;

				var dm = new DynamicMethod("Set" + field.Name, null, new[] { targetType, valueType }, targetType);

				var il = dm.GetILGenerator();
				// Load the instance of the object (argument 0) onto the stack
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldarg_1);
				// Load the value of the object's field (fi) onto the stack
#if NETCORE
				il.Emit(OpCodes.Callvirt, field.SetMethod);
#else
				il.Emit(OpCodes.Callvirt, field.GetSetMethod());
#endif
				// return the value on the top of the stack
				il.Emit(OpCodes.Ret);

				var delegateType = typeof(Action<,>).MakeGenericType(targetType, valueType);
				var getterDelegate = dm.CreateDelegate(delegateType);
				return getterDelegate;
			});
			return result;
		}


		#region all parameters is object type

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static Func<object, object> MakeObjectFieldGetter(FieldInfo field)
		{
			var result = ObjectGetterCache.GetOrAdd(field, (MemberInfo m) =>
			{
				var valueType = field.FieldType;
				var targetType = field.DeclaringType;

				var dm = new DynamicMethod("Get" + field.Name, typeof(object), new[] { typeof(object) }, targetType);
				var il = dm.GetILGenerator();
				// Load the instance of the object (argument 0) onto the stack
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Castclass, targetType);
				// Load the value of the object's field (fi) onto the stack
				il.Emit(OpCodes.Ldfld, field);
				if (valueType
#if NETCORE
					.GetTypeInfo()
#endif
					.IsValueType)
				{
					il.Emit(OpCodes.Box, valueType);
				}
				// return the value on the top of the stack
				il.Emit(OpCodes.Ret);

				var delegateType = typeof(Func<object, object>);
				var getterDelegate = (Func<object, object>)dm.CreateDelegate(delegateType);
				return getterDelegate;
			});
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static Action<object, object> MakeObjectFieldSetter(FieldInfo field)
		{
			var result = ObjectSetterCache.GetOrAdd(field, m =>
			{
				var valueType = field.FieldType;
				var targetType = field.DeclaringType;

				var dm = new DynamicMethod("Set" + field.Name, null, new[] { typeof(object), typeof(object) }, targetType);

				var il = dm.GetILGenerator();
				// Load the instance of the object (argument 0) onto the stack
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Castclass, targetType);
				il.Emit(OpCodes.Ldarg_1);
				if (valueType
#if NETCORE
					.GetTypeInfo()
#endif
				.IsValueType)
				{
					il.Emit(OpCodes.Unbox_Any, valueType);
				}
				// Load the value of the object's field (fi) onto the stack
				il.Emit(OpCodes.Stfld, field);
				// return the value on the top of the stack
				il.Emit(OpCodes.Ret);

				var delegateType = typeof(Action<object, object>);
				var getterDelegate = (Action<object, object>)dm.CreateDelegate(delegateType);
				return getterDelegate;
			});
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static Func<object, object> MakeObjectPropertyGetter(PropertyInfo field)
		{
			var result = ObjectGetterCache.GetOrAdd(field, (MemberInfo m) =>
			{
				var valueType = field.PropertyType;
				var targetType = field.DeclaringType;

				var dm = new DynamicMethod("Get" + field.Name, typeof(object), new[] { typeof(object) }, targetType);
				var il = dm.GetILGenerator();
				// Load the instance of the object (argument 0) onto the stack
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Castclass, targetType);
				// Load the value of the object's field (fi) onto the stack
#if NETCORE
				il.Emit(OpCodes.Callvirt, field.GetMethod);
#else
				il.Emit(OpCodes.Callvirt, field.GetGetMethod());
#endif
				if (valueType
#if NETCORE
					.GetTypeInfo()
#endif
					.IsValueType)
				{
					il.Emit(OpCodes.Box, valueType);
				}
				// return the value on the top of the stack
				il.Emit(OpCodes.Ret);

				var delegateType = typeof(Func<object, object>);
				var getterDelegate = (Func<object, object>)dm.CreateDelegate(delegateType);
				return getterDelegate;
			});
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public static Action<object, object> MakeObjectPropertySetter(PropertyInfo field)
		{
			var result = ObjectSetterCache.GetOrAdd(field, m =>
			{
				var valueType = field.PropertyType;
				var targetType = field.DeclaringType;

				var dm = new DynamicMethod("Set" + field.Name, null, new[] { typeof(object), typeof(object) }, targetType);

				var il = dm.GetILGenerator();
				// Load the instance of the object (argument 0) onto the stack
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Castclass, targetType);
				il.Emit(OpCodes.Ldarg_1);
				if (valueType
#if NETCORE
					.GetTypeInfo()
#endif
					.IsValueType)
				{
					il.Emit(OpCodes.Unbox_Any, valueType);
				}
				// Load the value of the object's field (fi) onto the stack
#if NETCORE
				il.Emit(OpCodes.Callvirt, field.SetMethod);
#else
				il.Emit(OpCodes.Callvirt, field.GetSetMethod());
#endif
				// return the value on the top of the stack
				il.Emit(OpCodes.Ret);

				var delegateType = typeof(Action<object, object>);
				var getterDelegate = (Action<object, object>)dm.CreateDelegate(delegateType);
				return getterDelegate;
			});
			return result;
		}

		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static FieldInfo GetField(Type type, string name)
		{
			return type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static PropertyInfo GetProperty(Type type, string name)
		{
			return type.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

	}

}
