using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PubNet.API.Abstractions;

namespace PubNet.API.Services;

public class ActionTemplateGenerator(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider) : IActionTemplateGenerator
{
	public string GetActionRoute(string controllerName, string actionName)
	{
		var normalizedControllerName = controllerName.EndsWith("Controller") ? controllerName[..^"Controller".Length] : controllerName;
		var normalizedActionName = actionName.EndsWith("Async") ? actionName[..^"Async".Length] : actionName;

		var endpoint = actionDescriptorCollectionProvider.ActionDescriptors.Items
			.Where(d => d is ControllerActionDescriptor)
			.Cast<ControllerActionDescriptor>()
			.Where(c => c.ControllerName == normalizedControllerName)
			.First(c => c.ActionName == normalizedActionName);

		return endpoint.AttributeRouteInfo?.Template ?? throw new InvalidOperationException("Endpoint has no route");
	}
}
