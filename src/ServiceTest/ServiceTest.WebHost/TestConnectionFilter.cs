using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.AspNetCore.Server.Kestrel.Filter;

namespace ServiceTest.WebHost
{
	public class TestConnectionFilter : IConnectionFilter
	{
		private readonly IConnectionFilter _previous;

		public TestConnectionFilter(IConnectionFilter previous)
		{
			if (previous == null)
			{
				throw new ArgumentNullException(nameof(previous));
			}

			_previous = previous;
		}

		public async Task OnConnectionAsync(ConnectionFilterContext context)
		{
			await _previous.OnConnectionAsync(context);

			if (string.Equals(context.Address.Scheme, "https", StringComparison.OrdinalIgnoreCase))
			{
			}
		}

	}

	public static class HttpsConnectionFilterExtensionFunc
	{
		public static KestrelServerOptions UseHttpsTest(this KestrelServerOptions options)
		{
			var prevFilter = options.ConnectionFilter ?? new NoOpConnectionFilter();
			options.ConnectionFilter = new TestConnectionFilter(prevFilter);
			return options;
		}

	}
}
