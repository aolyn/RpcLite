using ServiceRegistry.Domain.Model;

namespace ServiceRegistry.Management.ViewModels
{
	public class GroupIndexViewModel
	{
		public string ErrorMessage { get; set; }
		public ServiceGroup[] Groups { get; set; }
	}
}
