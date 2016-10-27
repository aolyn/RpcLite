using System;
using System.Diagnostics;
using System.IO;

namespace NuGetPackTool
{
	public class CommandLineHelper
	{
		public static void RunCommand(string args)
		{
			var shellFile = Guid.NewGuid().ToString() + ".cmd";
			File.WriteAllText(shellFile, args);
			var proc = Process.Start(shellFile);
			proc.WaitForExit();
			File.Delete(shellFile);
		}

	}
}
