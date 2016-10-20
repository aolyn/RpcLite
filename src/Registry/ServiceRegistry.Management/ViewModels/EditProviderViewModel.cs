using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceRegistry.Domain.Model;

namespace ServiceRegistry.Management.ViewModels
{
	public class EditProviderViewModel
	{
		public List<SelectListItem> GroupItems { get; internal set; }
		public ServiceProvider Provider { get; set; }
		public int ServiceId { get; set; }
	}
}
