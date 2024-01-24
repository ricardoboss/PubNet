using PubNet.Web.Abstractions.Models;

namespace PubNet.Web.Abstractions.Services;

public interface IGuardThrowsBuilder
{
	public void Cannot(Scope scope, string? message = null);
}
