using System;
using System.Collections.Generic;

namespace RpcLite
{
	internal class ServiceInstancePool
	{
		private static Dictionary<ActionInfo, ServiceInstanceContainer> containers = new Dictionary<ActionInfo, ServiceInstanceContainer>();
		private static object containerLock = new object();

		public static ServiceInstance GetServiceObject(ActionInfo action)
		{
			ServiceInstanceContainer container;
			if (containers.TryGetValue(action, out container))
				return container.GetServiceInstance();

			lock (containerLock)
			{
				container = new ServiceInstanceContainer(action.ServiceCreator) { Size = 1000 };
				containers.Add(action, container);
				return container.GetServiceInstance();
			}
		}
	}

	internal class ServiceInstanceContainer : IDisposable
	{
		private Func<object> serviceCreator;

		public static readonly Stack<ServiceInstance> Pool = new Stack<ServiceInstance>();
		public int Size { get; set; }

		public ServiceInstanceContainer(Func<object> serviceCreator)
		{
			this.serviceCreator = serviceCreator;
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		private object poolLock = new object();
		public ServiceInstance GetServiceInstance()
		{
			lock (poolLock)
			{
				if (Pool.Count > 0)
					return Pool.Pop();

				var service = serviceCreator();
				var instance = new ServiceInstance(this, service);
				return instance;
			}
		}

		public void ReleaseInstance(ServiceInstance instance)
		{
			lock (poolLock)
			{
				if (Pool.Count <= Size)
				{
					Pool.Push(instance);
				}
				else if (instance.ServiceObject is IDisposable)
				{
					((IDisposable)instance.ServiceObject).Dispose();
				}
			}
		}
	}

	internal class ServiceInstance : IDisposable
	{
		private readonly ServiceInstanceContainer _container;
		public readonly object ServiceObject;

		public ServiceInstance(ServiceInstanceContainer container, object serviceObject)
		{
			_container = container;
			ServiceObject = serviceObject;
		}

		public void Dispose()
		{
			_container.ReleaseInstance(this);
		}
	}
}
