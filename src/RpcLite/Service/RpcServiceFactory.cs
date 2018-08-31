using System.Collections.Generic;
using RpcLite.Config;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcServiceFactory
	{
		private readonly List<RpcService> _services = new List<RpcService>();
		private readonly AppHost _appHost;
		private readonly ServiceConfig _config;
		private readonly IServiceMapper _serviceMapper;

		/// <summary>
		/// 
		/// </summary>
		public IReadOnlyCollection<RpcService> Services { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="host"></param>
		/// <param name="config"></param>
		public RpcServiceFactory(AppHost host, ServiceConfig config)
		{
			_appHost = host;
			_config = config;
			Services = new ReadOnlyListWraper<RpcService>(_services);

			if (config?.Mapper != null)
			{
				var factory = ReflectHelper.CreateInstanceByIdentifier<IServiceMapperFactory>(config.Mapper.Type);
				_serviceMapper = factory.CreateServiceMapper(this, config);
			}
			else
			{
				_serviceMapper = new DefaultServiceMapper(this);
			}

			Initialize();
		}

		/// <summary>
		/// 
		/// </summary>
		private void Initialize()
		{
			if (_config?.Services == null)
				return;

			foreach (var item in _config.Services)
			{
				var typeInfo = ReflectHelper.GetTypeByIdentifier(item.Type);
				if (typeInfo == null)
				{
					throw new ServiceException("can't load service type: " + item.Type);
				}

				_services.Add(new RpcService(typeInfo, _appHost)
				{
					Name = item.Name,
					Path = item.Path,
					//Type = typeInfo,
					//Filters = _host.Filters,
				});
			}
		}

		/// <summary>
		/// <para>determinate if the request path is Service path</para>
		/// <para>get and set Service to serviceContext</para>
		/// <para>compute and set ActionName to serviceContext.Request</para>
		/// </summary>
		/// <param name="serviceContext"></param>
		/// <returns></returns>
		public MapServiceResult MapService(ServiceContext serviceContext)
		{
			return _serviceMapper.MapService(serviceContext);
		}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="name"></param>
		///// <returns></returns>
		//public static RpcService GetServiceByName(string name)
		//{
		//	var service = Services
		//		.FirstOrDefault(it => string.Compare(it.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
		//	return service;
		//}
	}

}
