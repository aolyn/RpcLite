using System;
using System.Linq;
using RpcLite.Logging;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class DefaultServiceMapper : IServiceMapper
	{
		private readonly RpcServiceFactory _factory;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="factory"></param>
		public DefaultServiceMapper(RpcServiceFactory factory)
		{
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));

			_factory = factory;
		}

		/// <summary>
		/// <para>determinate if the request path is Service path</para>
		/// <para>if yes get and set Service to serviceContext</para>
		/// <para>compute and set ActionName to serviceContext.Request</para>
		/// </summary>
		/// <param name="requestPath"></param>
		/// <param name="pathInfo"></param>
		/// <returns></returns>
		public MapServiceResult MapService(string requestPath, RequestPathInfo pathInfo)
		{
			var result = new MapServiceResult
			{
				IsServiceRequest = false,
			};

			if (pathInfo == null && string.IsNullOrWhiteSpace(requestPath))
			{
				//throw new ArgumentException("request.AppRelativeCurrentExecutionFilePath is null or white space");
				return result;
			}

			if (pathInfo != null)
			{
				var service = _factory.Services.FirstOrDefault(it =>
					pathInfo.Service.Equals(it.Name, StringComparison.OrdinalIgnoreCase));

				if (service == null)
				{
					LogHelper.Debug("BeginProcessReques Can't find service " + requestPath);
					//throw new ServiceNotFoundException(requestPath);
					return result;
				}

				result.ActionName = pathInfo.Action;
				result.Service = service;

				result.IsServiceRequest = true;
				return result;
			}
			else
			{
				var service = _factory.Services.FirstOrDefault(it =>
					requestPath.StartsWith(it.Path, StringComparison.OrdinalIgnoreCase));

				if (service == null)
				{
					service = _factory.Services.FirstOrDefault(it =>
						it.Path.StartsWith(requestPath, StringComparison.OrdinalIgnoreCase));
					if (service == null)
					{
						LogHelper.Debug("BeginProcessReques Can't find service " + requestPath);
						//throw new ServiceNotFoundException(requestPath);
						return result;
					}
				}
				else
				{
					var actionName = requestPath.Substring(service.Path.Length);
					result.ActionName = actionName;
				}
				result.Service = service;

				result.IsServiceRequest = true;
				return result;
			}
		}
	}

}
