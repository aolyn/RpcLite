using System;
using System.IO;
using System.Text.RegularExpressions;

namespace NuGetPackTool
{
	class Program
	{
		/// <summary>
		/// Application Entry Point.
		/// </summary>
		[STAThread]
		public static void Main(string[] args)
		{
			if (args.Length == 2 && args[0] == "addver")
			{
				Console.WriteLine("addver");
				//var file = @"D:\git-repo\git-hub\RpcLite-chrishaly\src\Aolyn\Aolyn.Data.Npgsql\Properties\AssemblyInfo.cs";
				//AddAssemblyVersion(file);
				AddAssemblyVersion(args[1]);
				return;
			}

			var app = new App();
			app.InitializeComponent();
			app.Run();
		}

		private static void AddAssemblyVersion(string file)
		{
			var reg = @"AssemblyVersion\(""([0-9.]+)""\)";
			var text = File.ReadAllText(file);
			var result = Regex.Replace(text, reg, m =>
			{
				var version = new Version(m.Groups[1].Value);
				var newVersion = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision + 1}";
				return $"AssemblyVersion(\"{newVersion}\")"; // m.Groups[1].Value + newVersion + m.Groups[2].Value;
			});
			File.WriteAllText(file, result);
		}
	}
}