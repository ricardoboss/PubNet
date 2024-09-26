namespace PubNet.API.Abstractions;

public interface IActionTemplateGenerator
{
	string GetActionRoute(string controllerName, string actionName);
}
