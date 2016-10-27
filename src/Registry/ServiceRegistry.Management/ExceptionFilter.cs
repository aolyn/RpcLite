using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using ServiceRegistry.Management.ViewModels;

namespace ServiceRegistry.Management
{
	public class CustomExceptionFilter : IExceptionFilter
	{
		private readonly IHostingEnvironment _hostingEnvironment;
		private readonly IModelMetadataProvider _modelMetadataProvider;

		public CustomExceptionFilter(
			IHostingEnvironment hostingEnvironment,
			IModelMetadataProvider modelMetadataProvider)
		{
			_hostingEnvironment = hostingEnvironment;
			_modelMetadataProvider = modelMetadataProvider;
		}

		public void OnException(ExceptionContext context)
		{
			//if (!_hostingEnvironment.IsDevelopment())
			//{
			//	// do nothing
			//	return;
			//}

			var model = new ErrorPageViewModel
			{
				ErrorMessage = context.Exception.Message
			};

			var viewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);

			var result = new ViewResult
			{
				ViewName = "ErrorFilter",
				ViewData = new ViewDataDictionary<ErrorPageViewModel>(viewData, model)
			};
			context.ExceptionHandled = true; // mark exception as handled
			context.Result = result;
		}
	}
}