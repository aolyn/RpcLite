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
			var repo = new ServiceRepository();
			var services = await repo.GetAllAsync();
			var model = new ServiceListComponentViewModel
			{
				Services = services,
			};
			repo.Dispose();

			return View(model);
		}

	}
}
