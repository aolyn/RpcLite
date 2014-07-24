using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using Model;

namespace DynamicProxyTest
{
	public interface IRandom
	{
		int Number { get; set; }
	}

	public class TestClass
	{
		public static void Test()
		{
			try
			{
				IRandom random = ConvertAnonymousTypeToInterface<IRandom>(new { Number = 3 });
				Console.WriteLine("Value of random = " + random.Number.ToString());
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}

			Console.ReadLine();
		}

		static T ConvertAnonymousTypeToInterface<T>(object data) where T : class
		{
			// A little error checking
			if (!typeof(T).IsInterface)
				throw new InvalidOperationException("T must be an interface");

			if (!typeof(T).IsPublic)
				throw new InvalidOperationException("T must be a public interface");

			string typeName = typeof(T).Name;

			// We need an assembly to generate our type in
			AssemblyName assemblyName = new AssemblyName("AnonymousInterfaceAssembly");
			assemblyName.Version = new Version("1.0.0.0");

			// We need a few builders before we can build a type:
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly
			    (assemblyName, AssemblyBuilderAccess.RunAndSave);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule
			    ("AnonymousInterfaceAssembly", "AnonymousInterfaceAssembly.dll");
			TypeBuilder typeBuilder = moduleBuilder.DefineType
			    ("AnonymousInterfaceAssembly." + typeName, TypeAttributes.Public, typeof(object));

			// Now we can get on with building an object that implements the interface.
			// First we'll tell the runtime we want to implement the interface passed in
			typeBuilder.AddInterfaceImplementation(typeof(T));

			// We'll be fine with a default constructor
			typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

			// Implement interface members :o

			// Pull the properties & loop through them
			PropertyDescriptorCollection interfaceProperties =
			    TypeDescriptor.GetProperties(typeof(T));

			foreach (PropertyDescriptor interfaceProperty in interfaceProperties)
			{
				// First we'll define a new field to hold our property
				FieldBuilder propertyFieldBuilder = typeBuilder.DefineField
				    ("_" + typeName, interfaceProperty.PropertyType, FieldAttributes.Private);

				// Our get & set methods need a few attributes
				MethodAttributes getSetMethodAttributes =
				    MethodAttributes.Public | MethodAttributes.Virtual;

				// implementing our inteface requires a get method named 'get_PROPERTYNAME' 
				// ( where PROPERTYNAME is really the property name. )
				MethodBuilder propertyGetMethod = typeBuilder.DefineMethod(
				    "get_" + interfaceProperty.Name, getSetMethodAttributes,
				    interfaceProperty.PropertyType, Type.EmptyTypes);

				ILGenerator getMethodGenerator = propertyGetMethod.GetILGenerator();

				// our get consists of loading the field & returning
				getMethodGenerator.Emit(OpCodes.Ldarg_0);
				getMethodGenerator.Emit(OpCodes.Ldfld, propertyFieldBuilder);
				getMethodGenerator.Emit(OpCodes.Ret);

				// that's it for the get method, on to the set
				MethodBuilder propertySetMethod = typeBuilder.DefineMethod(
				    "set_" + interfaceProperty.Name, getSetMethodAttributes,
				    null, new Type[] { interfaceProperty.PropertyType });

				ILGenerator setMethodGenerator = propertySetMethod.GetILGenerator();

				// The code is basically the same ( just in the other direction ), 
				// however we'll have an actual argument coming in ( to set the value )
				setMethodGenerator.Emit(OpCodes.Ldarg_0);
				setMethodGenerator.Emit(OpCodes.Ldarg_1);
				setMethodGenerator.Emit(OpCodes.Stfld, propertyFieldBuilder);
				setMethodGenerator.Emit(OpCodes.Ret);

				// And we're finished!
			}

			// Let's build our object

			Type anonymousInterfaceType = typeBuilder.CreateType();

			// set values

			T instance = Activator.CreateInstance(anonymousInterfaceType) as T;

			foreach (PropertyDescriptor interfaceProperty in interfaceProperties)
			{
				PropertyInfo dataProperty = data.GetType().GetProperty(interfaceProperty.Name);
				object value = dataProperty.GetValue(data, null);

				// Find the set method we just built:
				MethodInfo setterMethod = anonymousInterfaceType.GetMethod(
				    "set_" + interfaceProperty.Name);

				setterMethod.Invoke(instance, new object[] { value });
			}

			return instance;
		}
	}

	public class TestClass3
	{
		public TestClass3(int a, string b, Product c, int d, int e, int f)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this._D = d;
			this._E = e;
			this._F = f;
		}

		public void SetValues(int a, string b, Product c, int d2, int e, int f)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this._D = d2;
			this._E = e;
			this._F = f;
		}

		public void SetValues222(int a, string b, Product c, int d2, int e, int f)
		{

		}

		private int a;
		public int A
		{
			get { return a; }
			set { a = value; }
		}

		private string b;
		public string B
		{
			get { return b; }
			set { b = value; }
		}

		private Product c;
		public Product C
		{
			get { return c; }
			set { c = value; }
		}

		private int _D = 0;
		public int D { get { return _D; } set { _D = value; } }

		private int _E = 0;
		public int E { get { return _E; } set { _E = value; } }

		private int _F = 0;
		public int F { get { return _F; } set { _F = value; } }

		private int _G = 0;
		public int G { get { return _G; } set { _G = value; } }
	}
}
