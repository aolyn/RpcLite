using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace WebApiClient
{
	class AssemblyBuilderDemo
	{
		public static Type BuildDynAssembly()
		{

			Type pointType = null;

			AppDomain currentDom = Thread.GetDomain();

			Console.Write("Please enter a name for your new assembly: ");
			StringBuilder asmFileNameBldr = new StringBuilder();
			asmFileNameBldr.Append(Console.ReadLine());
			asmFileNameBldr.Append(".exe");
			string asmFileName = asmFileNameBldr.ToString();

			AssemblyName myAsmName = new AssemblyName();
			myAsmName.Name = "MyDynamicAssembly";

			AssemblyBuilder myAsmBldr = currentDom.DefineDynamicAssembly(
				myAsmName,
				AssemblyBuilderAccess.RunAndSave);

			// We've created a dynamic assembly space - now, we need to create a module
			// within it to reflect the type Point into.

			ModuleBuilder myModuleBldr = myAsmBldr.DefineDynamicModule(asmFileName,
				asmFileName);

			TypeBuilder myTypeBldr = myModuleBldr.DefineType("Point");

			FieldBuilder xField = myTypeBldr.DefineField("x", typeof(int),
				FieldAttributes.Private);
			FieldBuilder yField = myTypeBldr.DefineField("y", typeof(int),
				FieldAttributes.Private);

			pointType = myTypeBldr.CreateType();

			Console.WriteLine("Type completed.");

			//myAsmBldr.SetEntryPoint(pointMainBldr);

			myAsmBldr.Save(asmFileName);

			Console.WriteLine("Assembly saved as '{0}'.", asmFileName);
			Console.WriteLine("Type '{0}' at the prompt to run your new " +
							  "dynamically generated dot product calculator.",
				asmFileName);

			// After execution, this program will have generated and written to disk,
			// in the directory you executed it from, a program named 
			// <name_you_entered_here>.exe. You can run it by typing
			// the name you gave it during execution, in the same directory where
			// you executed this program.

			return pointType;

		}

		public static void Main22()
		{

			Type myType = BuildDynAssembly();
			Console.WriteLine("---");

			// Let's invoke the type 'Point' created in our dynamic assembly. 

			object ptInstance = Activator.CreateInstance(myType, new object[] { 0, 0 });

			myType.InvokeMember("PointMain",
				BindingFlags.InvokeMethod,
				null,
				ptInstance,
				new object[0]);


		}
	}

	// The Point class is the class we will reflect on and copy into our
	// dynamic assembly. The public static function PointMain() will be used
	// as our entry point.
	//
	// We are constructing the type seen here dynamically, and will write it
	// out into a .exe file for later execution from the command-line.
	// --- 
	// class Point {
	//   
	//   private int x;
	//   private int y;
	//
	//   public Point(int ix, int iy) {
	//
	//   	this.x = ix;
	//    	this.y = iy;
	//
	//   }
	//
	//   public int DotProduct (Point p) {
	//   
	//   	return ((this.x * p.x) + (this.y * p.y));
	//
	//   }
	//
	//   public static void PointMain() {
	//     
	//     Console.Write("Enter the 'x' value for point 1: "); 
	//     int x1 = Convert.ToInt32(Console.ReadLine());
	//     
	//     Console.Write("Enter the 'y' value for point 1: ");
	//     int y1 = Convert.ToInt32(Console.ReadLine());
	//
	//     Console.Write("Enter the 'x' value for point 2: "); 
	//     int x2 = Convert.ToInt32(Console.ReadLine());
	//     
	//     Console.Write("Enter the 'y' value for point 2: ");
	//     int y2 = Convert.ToInt32(Console.ReadLine());
	//
	//     Point p1 = new Point(x1, y1);
	//     Point p2 = new Point(x2, y2);
	//
	//     Console.WriteLine("({0}, {1}) . ({2}, {3}) = {4}.",
	//		       x1, y1, x2, y2, p1.DotProduct(p2));
	//   
	//   }
	//
	// }
	// ---
}

