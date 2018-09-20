using RpcLite.Service;

#if NETCORE
using System.Diagnostics;
#endif

namespace RpcLite.Diagnostics
{
	internal class DiagnosticsLogger
	{
		private const string ComponentName = "Aolyn.RpcLite";
		private const string ServiceRequestStart = "Service.RequestStart";
		private const string ServiceRequestEnd = "Service.RequestEnd";
		private const string ClientRequestStart = "Client.RequestStart";
		private const string ClientRequestEnd = "Client.RequestEnd";

		private static readonly DiagnosticListener DiagnosticSource = new DiagnosticListener(ComponentName);

		public static void WriteServiceRequestStart(ServiceContext serverContext)
		{
			if (DiagnosticSource.IsEnabled(ServiceRequestStart))
			{
				DiagnosticSource.Write(ServiceRequestStart, serverContext);
			}
		}

		public static void WriteServiceRequestEnd(ServiceContext serverContext)
		{
			if (DiagnosticSource.IsEnabled(ServiceRequestEnd))
			{
				DiagnosticSource.Write(ServiceRequestEnd, serverContext);
			}
		}

		public static void WriteClientRequestStart(object serverContext)
		{
			if (DiagnosticSource.IsEnabled(ClientRequestStart))
			{
				DiagnosticSource.Write(ClientRequestStart, serverContext);
			}
		}

		public static void WriteClientRequestEnd(object serverContext)
		{
			if (DiagnosticSource.IsEnabled(ClientRequestEnd))
			{
				DiagnosticSource.Write(ClientRequestEnd, serverContext);
			}
		}


#if !NETCORE
		class DiagnosticListener
		{
			// ReSharper disable UnusedParameter.Local
			public DiagnosticListener(string componentName)
			{
			}

			public bool IsEnabled(string serviceRequestStart)
			{
				return false;
			}

			public void Write(string serviceRequestStart, object serverContext)
			{
			}
			// ReSharper restore UnusedParameter.Local
		}
#endif

	}
}
