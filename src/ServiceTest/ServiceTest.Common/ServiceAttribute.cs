using System;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceTest.Common
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class ServiceAttribute : Attribute
	{
		public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;

		public ServiceAttribute() { }

		public ServiceAttribute(ServiceLifetime lifetime)
		{
			Lifetime = lifetime;
		}
	}
}