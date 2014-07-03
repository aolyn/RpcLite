using System;
using System.Collections.Generic;

namespace RpcLite
{
	internal class ServiceFactory
	{
		private static Dictionary<ActionInfo, ServiceInstancePool> containers = new Dictionary<ActionInfo, ServiceInstancePool>();
		private static object containerLock = new object();

		public static ServiceInstanceContainer GetService(ActionInfo action)
		{
			if (action == null) throw new ArgumentNullException("action can't be null");
			if (action.ServiceCreator == null) throw new ArgumentNullException("action.ServiceCreator can't be null");

			//var serviceCreator = action.ServiceCreator;
			ServiceInstancePool container;
			if (containers.TryGetValue(action, out container))
				return container.GetInstance();

			lock (containerLock)
			{
				container = new ServiceInstancePool(action.ServiceCreator) { Size = 1000 };
				containers.Add(action, container);
				return container.GetInstance();
			}
		}
	}

	internal class ServiceInstancePool : IDisposable
	{
		private Func<object> serviceCreator;

		public static readonly Stack<ServiceInstanceContainer> Pool = new Stack<ServiceInstanceContainer>();
		public int Size { get; set; }

		public ServiceInstancePool(Func<object> serviceCreator)
		{
			this.serviceCreator = serviceCreator;
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		private object poolLock = new object();
		public ServiceInstanceContainer GetInstance()
		{
			lock (poolLock)
			{
				if (Pool.Count > 0)
					return Pool.Pop();

				var service = serviceCreator();
				var instance = new ServiceInstanceContainer(this, service);
				return instance;
			}
		}

		public void ReleaseInstance(ServiceInstanceContainer instance)
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

	internal class ServiceInstanceContainer : IDisposable
	{
		private readonly ServiceInstancePool _container;
		public readonly object ServiceObject;

		public ServiceInstanceContainer(ServiceInstancePool container, object serviceObject)
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
