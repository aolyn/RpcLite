using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpcLite.Logging
{
	internal static class LogHelper
	{
		public static void Debug(string message)
		{

		}

		public static void Error(string v, Exception ex)
		{

		}

		internal static void Error(Exception ex)
		{
			throw new NotImplementedException();
		}
	}
}
