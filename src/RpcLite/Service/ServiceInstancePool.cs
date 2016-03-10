using System;
using System.Collections.Generic;

namespace RpcLite.Service
{
	internal class ServiceFactory
	{
		private static readonly QuickReadConcurrentDictionary<RpcAction, ServiceInstanceContainerPool> Pool = new QuickReadConcurrentDictionary<RpcAction, ServiceInstanceContainerPool>();
		//private static readonly object ContainerLock = new object();

		public static ServiceInstanceContainer GetService(RpcAction action)
		{
			if (action == null) throw new ArgumentNullException(nameof(action));
			if (action.ServiceCreator == null) throw new ArgumentException("action.ServiceCreator can't be null");

			//var serviceCreator = action.ServiceCreator;
			//ServiceInstanceContainerPool container;
			var container = Pool.GetOrAdd(action, () =>
				new ServiceInstanceContainerPool(action.ServiceCreator) { Size = 1000 });
			return container.GetInstanceContainer();

			//if (Pool.TryGetValue(action, out container))
			//	return container.GetInstanceContainer();

			//lock (ContainerLock)
			//{
			//	if (Pool.TryGetValue(action, out container))
			//		return container.GetInstanceContainer();

			//	container = new ServiceInstanceContainerPool(action.ServiceCreator) { Size = 1000 };
			//	Pool.Add(action, container);
			//	return container.GetInstanceContainer();
			//}
		}
	}

	internal class ServiceInstanceContainerPool : IDisposable
	{
		private readonly Func<object> _serviceCreator;

		public static readonly Stack<ServiceInstanceContainer> Pool = new Stack<ServiceInstanceContainer>();
		public int Size { get; set; }

		public ServiceInstanceContainerPool(Func<object> serviceCreator)
		{
			if (serviceCreator == null)
				throw new ArgumentNullException(nameof(serviceCreator));

			_serviceCreator = serviceCreator;
		}

		public void Dispose()
		{
			Pool.Clear();
		}

		private readonly object _poolLock = new object();
		public ServiceInstanceContainer GetInstanceContainer()
		{
			lock (_poolLock)
			{
				if (Pool.Count > 0)
					return Pool.Pop();

				var serviceInstance = _serviceCreator();
				var container = new ServiceInstanceContainer(this, serviceInstance);
				return container;
			}
		}

		public void ReleaseInstanceContainer(ServiceInstanceContainer instance)
		{
			lock (_poolLock)
			{
				if (Pool.Count <= Size)
				{
					Pool.Push(instance);
				}
				else
				{
					var o = instance.ServiceObject as IDisposable;
					if (o != null)
					{
						o.Dispose();
					}
				}
			}
		}
	}

	internal class ServiceInstanceContainer : IDisposable
	{
		private readonly ServiceInstanceContainerPool _container;
		public readonly object ServiceObject;

		public ServiceInstanceContainer(ServiceInstanceContainerPool container, object serviceObject)
		{
			_container = container;
			ServiceObject = serviceObject;
		}

		public void Dispose()
		{
			_container.ReleaseInstanceContainer(this);
		}
	}
}
