using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PubNet.API.Abstractions;

namespace PubNet.API.Services;

public class ActionTemplateGenerator(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider) : IActionTemplateGenerator
{
	public string GetActionRoute(string controllerName, string actionName)
	{
		var endpoint = actionDescriptorCollectionProvider.ActionDescriptors.Items
			.Where(d => d is ControllerActionDescriptor)
			.Cast<ControllerActionDescriptor>()
			.Where(c => c.ControllerName == controllerName[..^"Controller".Length])
			.First(c => c.ActionName == actionName[..^"Async".Length]);

		return endpoint.AttributeRouteInfo?.Template ?? throw new InvalidOperationException("Endpoint has no route");
	}
}
