using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceRegistry.Management.ViewModels;
using ServiceRegistry.Repositories;

namespace ServiceRegistry.Management.ViewComponent
{
	public class ServiceListViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var model = new ServiceListComponentViewModel();
			try
			{
				using (var repo = new ServiceRepository())
				{
					var services = await repo.GetAllAsync();
					model.Services = services;
				}
			}
			catch (Exception ex)
			{
				model.ErrorMessage = "Get Service List Error: " + ex.Message;
			}

			return View(model);
		}

	}
}
