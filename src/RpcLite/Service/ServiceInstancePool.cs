using System;
using System.Collections.Generic;

namespace RpcLite.Service
{
	internal class ServiceFactory
	{
		private static readonly Dictionary<RpcAction, ServiceInstanceContainerPool> pool = new Dictionary<RpcAction, ServiceInstanceContainerPool>();
		private static readonly object containerLock = new object();

		public static ServiceInstanceContainer GetService(RpcAction action)
		{
			if (action == null) throw new ArgumentNullException("action");
			if (action.ServiceCreator == null) throw new ArgumentException("action.ServiceCreator can't be null");

			//var serviceCreator = action.ServiceCreator;
			ServiceInstanceContainerPool container;
			if (pool.TryGetValue(action, out container))
				return container.GetInstanceContainer();

			lock (containerLock)
			{
				if (pool.TryGetValue(action, out container))
					return container.GetInstanceContainer();

				container = new ServiceInstanceContainerPool(action.ServiceCreator) { Size = 1000 };
				pool.Add(action, container);
				return container.GetInstanceContainer();
			}
		}
	}

	internal class ServiceInstanceContainerPool : IDisposable
	{
		private readonly Func<object> serviceCreator;

		public static readonly Stack<ServiceInstanceContainer> Pool = new Stack<ServiceInstanceContainer>();
		public int Size { get; set; }

		public ServiceInstanceContainerPool(Func<object> serviceCreator)
		{
			if (serviceCreator == null)
				throw new ArgumentNullException("serviceCreator");

			this.serviceCreator = serviceCreator;
		}

		public void Dispose()
		{
			Pool.Clear();
		}

		private readonly object poolLock = new object();
		public ServiceInstanceContainer GetInstanceContainer()
		{
			lock (poolLock)
			{
				if (Pool.Count > 0)
					return Pool.Pop();

				var serviceInstance = serviceCreator();
				var container = new ServiceInstanceContainer(this, serviceInstance);
				return container;
			}
		}

		public void ReleaseInstanceContainer(ServiceInstanceContainer instance)
		{
			lock (poolLock)
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
