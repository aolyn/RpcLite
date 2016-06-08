using System;
using System.Globalization;

namespace RpcLiteTestServiceWeb
{
	public class TestService
	{
		public string GetDateTimeString()
		{
			return DateTime.Now.ToString(CultureInfo.InvariantCulture);
		}
	}
}
