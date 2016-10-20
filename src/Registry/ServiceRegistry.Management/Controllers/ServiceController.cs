using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceRegistry.Management.ViewModels;
using ServiceRegistry.Repositories;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ServiceRegistry.Management.Controllers
{
	public class ServiceController : Controller
	{
		// GET: /<controller>/
		public async Task<IActionResult> Index()
		{
			var repo = new ServiceRepository();
			var services = await repo.GetAllAsync();
			var model = new ServiceIndexViewModel
			{
				Services = services,
			};
			repo.Dispose();

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> EditService(int id)
		{
			if (id != 0)
			{
				using (var repo = new ServiceRepository())
				{
					var service = await repo.GetByIdAsync(id);
					return View(new EditServiceViewModel
					{
						Service = service
					});
				}
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> EditService(EditServiceViewModel model)
		{
			if ((model.Service?.Id ?? 0) != 0)
			{
				using (var repo = new ServiceRepository())
				{
					await repo.UpdateAsync(model.Service);
					return View(model);
				}
			}

			if (!string.IsNullOrWhiteSpace(model.Service?.Name))
			{
				using (var repo = new ServiceRepository())
				{
					await repo.AddAsync(model.Service);
					return View(model);
				}
			}

			return View(model);
		}

		public async Task<IActionResult> Providers(int id)
		{
			var repo = new ServiceProviderRepository();
			var services = await repo.GetAllAsync(it => it.ServiceId == id);
			var model = new ServiceProvidersViewModel
			{
				ServiceProviers = services,
				ServiceId = id,
			};
			repo.Dispose();

			return View(model);
		}

		public async Task<IActionResult> EditProvider(int id)
		{
			var groupItems = await GetGroupItems();

			var repo = new ServiceProviderRepository();
			var provider = await repo.GetByIdAsync(id);
			var model = new EditProviderViewModel
			{
				Provider = provider,
				GroupItems = groupItems,
			};
			repo.Dispose();

			return View(model);
		}

		private static async Task<List<SelectListItem>> GetGroupItems()
		{
			var groupRepo = new ServiceGroupRepository();
			var groups = await groupRepo.GetAllAsync();
			var groupItems = groups
				.Select(it => new SelectListItem { Value = it.Id, Text = it.Id })
				.ToList();
			return groupItems;
		}

		[HttpPost]
		public async Task<IActionResult> EditProvider(EditProviderViewModel model, int serviceId)
		{
			var groupItems = await GetGroupItems();
			model.GroupItems = groupItems;

			if (model.Provider.Id == 0)
			{
				model.Provider.ServiceId = serviceId;
				using (var repo1 = new ServiceProviderRepository())
				{
					await repo1.AddAsync(model.Provider);
					return View(model);
				}
			}

			var repo = new ServiceProviderRepository();
			await repo.UpdateAsync(model.Provider);
			return View(model);
		}

		public async Task<IActionResult> DeleteService(int id)
		{
			try
			{
				var repo = new ServiceRepository();
				var service = await repo.GetByIdAsync(id);

				ViewBag.StatusMessage = $"Are you sure delete service {service.Name}?";
			}
			catch (Exception)
			{
				ViewBag.StatusMessage = "Get service info error.";
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> DeleteService(int id, bool confirm)
		{
			try
			{
				var repo = new ServiceRepository();
				await repo.RemoveAsync(id);

				ViewBag.StatusMessage = $"Service Deleted";
			}
			catch (Exception)
			{
				ViewBag.StatusMessage = "Deleted service error.";
			}

			return View();
		}

	}
}
