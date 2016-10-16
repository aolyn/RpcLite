using System;
using System.Linq;
using System.Threading.Tasks;
using RpcLite;
using ServiceRegistry.Contract;
using ServiceRegistry.Repositories;

namespace ServiceRegistry.Service.Services
{
	public class RegistryService : IRegistryService
	{
		public GetServiceAddressResponse GetServiceAddress(GetServiceAddressRequest request)
		{
			if (!(request?.Services?.Length > 0))
				return new GetServiceAddressResponse();

			using (var repository = new ServiceMappingRepository())
			{
				var results = request.Services
					.Select(item =>
					{
						try
						{
							var serviceMapping = repository.GetAsync(it => it.Service.Name == item.Name
								&& it.Environment == item.Group).Result;

							var addr = new ServiceInfoDto
							{
								Name = item.Name,
								Group = item.Group,
								Address = serviceMapping.Address,
								Data = null,
							};

							var result = new ResultDto
							{
								Identifier = item,
								ServiceInfos = new[] { addr }
							};

							return result;
						}
						catch (Exception ex)
						{
						}
						return null;
					})
					.Where(it => it != null)
					.ToArray();

				var response = new GetServiceAddressResponse
				{
					Results = results
				};

				return response;
			}
		}

		public Task<GetServiceAddressResponse> GetServiceAddressAsync(GetServiceAddressRequest request)
		{
			if (!(request?.Services?.Length > 0))
				return TaskHelper.FromResult(new GetServiceAddressResponse());

			var tcs = new TaskCompletionSource<GetServiceAddressResponse>();

			using (var repository = new ServiceMappingRepository())
			{
				var results = request.Services
					.Select(item => new
					{
						Identifier = item,
						Task = repository.GetAsync(it => it.Service.Name == item.Name
							&& it.Environment == item.Group)
					})
					.Select(item =>
					{
						if (item.Task.IsFaulted || item.Task.Result == null)
							return null;

						var serviceMapping = item.Task.Result;

						var addr = new ServiceInfoDto
						{
							Name = item.Identifier.Name,
							Group = item.Identifier.Group,
							Address = serviceMapping.Address,
							Data = null,
						};

						var result = new ResultDto
						{
							Identifier = item.Identifier,
							ServiceInfos = new[] { addr }
						};

						return result;
					})
					.Where(it => it != null)
					.ToArray();

				var response = new GetServiceAddressResponse
				{
					Results = results
				};
				tcs.SetResult(response);
			}

			return tcs.Task;
		}
	}
}