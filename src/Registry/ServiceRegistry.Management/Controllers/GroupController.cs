using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceRegistry.Domain.Model;
using ServiceRegistry.Management.ViewModels;
using ServiceRegistry.Repositories;

namespace ServiceRegistry.Management.Controllers
{
	public class GroupController : Controller
	{
		public async Task<IActionResult> Index()
		{
			var model = new GroupIndexViewModel();
			try
			{
				using (var repo = new ServiceGroupRepository())
				{
					var services = await repo.GetAllAsync();
					model.Groups = services;
				}
			}
			catch (Exception ex)
			{
				model.ErrorMessage = "Get Service List Error: " + ex.Message;
			}

			return View(model);
		}


		public async Task<IActionResult> EditGroup(string id)
		{
			var model = new EditGroupiewModel();
			if (id == null)
			{
				model.Group = new ServiceGroup();
				return View(model);
			}

			using (var repo = new ServiceGroupRepository())
			{
				var group = await repo.GetByIdAsync(id);
				model.Id = @group.Id;
				model.Group = new ServiceGroup
				{
					Id = @group.Id,
				};
				return View(model);
			}
		}

		[HttpPost]
		public async Task<IActionResult> EditGroup(EditGroupiewModel model)
		{
			if (model.Id != null)
			{
				using (var repo = new ServiceGroupRepository())
				{
					await repo.UpdateAsync(model.Group);
					return View(model);
				}
			}

			if (!string.IsNullOrWhiteSpace(model.Group?.Id))
			{
				using (var repo = new ServiceGroupRepository())
				{
					await repo.AddAsync(model.Group);
					model.Id = model.Group.Id;
					return View(model);
				}
			}

			return View(model);
		}

		public async Task<IActionResult> DeleteGroup(string id)
		{
			try
			{
				var repo = new ServiceGroupRepository();
				var service = await repo.GetByIdAsync(id);

				ViewBag.StatusMessage = $"Are you sure delete group {service.Id}?";
			}
			catch (Exception)
			{
				ViewBag.StatusMessage = "Get service info error.";
			}

			return View();
		}


		[HttpPost]
		public async Task<IActionResult> DeleteGroup(string id, bool confirm)
		{
			try
			{
				var repo = new ServiceGroupRepository();
				await repo.RemoveAsync(id);

				ViewBag.StatusMessage = $"group Deleted";
			}
			catch (Exception)
			{
				ViewBag.StatusMessage = "Deleted group error.";
			}

			return View();
		}

	}
}