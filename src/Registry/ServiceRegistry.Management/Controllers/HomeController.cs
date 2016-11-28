using System;
using Microsoft.AspNetCore.Mvc;

namespace ServiceRegistry.Management.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			//ViewData["Message"] = "Contact way.";

			return View();
		}

		public IActionResult Error(string id)
		{
			ViewBag.StatusCode = id;
			return View();
		}


		public IActionResult Throw(string id)
		{
			if (id == "99")
			{
				throw new InvalidOperationException("test exception");
			}

			return new JsonResult(new
			{
				Id = id
			});
		}

	}
}
