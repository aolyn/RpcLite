using System;
using System.Globalization;

namespace RpcLiteServiceTest
{
	public class TestService1
	{
		public string GetDateTimeString()
		{
			return DateTime.Now.ToString(CultureInfo.InvariantCulture);
		}
	}
}