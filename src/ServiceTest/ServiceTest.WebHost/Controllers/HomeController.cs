using Microsoft.AspNetCore.Mvc;

namespace ServiceTest.WebHost.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
