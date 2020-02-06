using System;
#pragma warning disable 1591

namespace RpcLite.Logging
{
	public static class LogHelper
	{
		public static void Debug(string message)
		{

		}

		public static void Debug(Exception exception)
		{

		}

		public static void Error(string message)
		{
			Console.WriteLine(message);
		}

		public static void Error(string msg, Exception ex)
		{
			Console.WriteLine(msg);
			Console.WriteLine(ex);
		}

		public static void Error(Exception ex)
		{
			Console.WriteLine(ex);
		}
	}
}
