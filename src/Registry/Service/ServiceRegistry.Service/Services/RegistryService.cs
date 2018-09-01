using System;
using System.Linq;
using System.Threading.Tasks;
using RpcLite.Logging;
using RpcLite.Registry.Merops.Contract;
using ServiceRegistry.Repositories;

namespace ServiceRegistry.Service.Services
{
	public class RegistryService : IRegistryService
	{
		public GetServiceInfoResponse GetServiceInfo(GetServiceInfoRequest request)
		{
			if (!(request?.Services?.Length > 0))
				return new GetServiceInfoResponse();

			var results = GetResultsAsync(request.Services);
			var response = new GetServiceInfoResponse
			{
				Services = results.Result
			};
			return response;
		}

		private static async Task<ServiceResultDto[]> GetResultsAsync(ServiceIdentifierDto[] identifiers)
		{
			using (var repository = new ServiceProviderRepository())
			{
				var tasks = identifiers
					.Select(async item =>
					{
						try
						{
							var serviceMapping = await repository.GetAllAsync(it => it.Service.Name == item.Name
								&& it.Group == item.Group).ConfigureAwait(false);

							var addr = serviceMapping
								.Select(it => new ServiceInfoDto
								{
									Name = item.Name,
									Group = it.Group,
									Address = it.Address,
									Data = it.Data,
								})
								.ToArray();

							var result = new ServiceResultDto
							{
								Identifier = item,
								ServiceInfos = addr,
							};

							return result;
						}
						catch (Exception ex)
						{
							LogHelper.Error(ex);
						}
						return null;
					})
					.ToArray();

				await Task.WhenAll(tasks).ConfigureAwait(false);

				var results = tasks
						.Select(it => it.Result)
						.Where(it => it != null)
						.ToArray();
				return results;
			}
		}

		public async Task<GetServiceInfoResponse> GetServiceInfoAsync(GetServiceInfoRequest request)
		{
			if (!(request?.Services?.Length > 0))
				return new GetServiceInfoResponse();

			var results = await GetResultsAsync(request.Services).ConfigureAwait(false);
			var response = new GetServiceInfoResponse
			{
				Services = results
			};
			return response;
		}
	}
}