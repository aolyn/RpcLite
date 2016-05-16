using System;
using System.Globalization;

namespace TestWebCoreFx
{
	public class TestService
	{
		public string GetDateTimeString()
		{
			return DateTime.Now.ToString(CultureInfo.InvariantCulture);
		}
	}
}
