using System.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using PubNet.API.Services;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Authorization;

/// <summary>
/// Authorizes admins by looking up their current role instead of trusting a claim, so a demotion takes effect
/// immediately even though tokens are long-lived and never expire.
/// </summary>
public sealed class AdminAuthorizationHandler(
	PubNetContext db,
	ApplicationRequestContext requestContext,
	ILogger<AdminAuthorizationHandler> logger
) : AuthorizationHandler<AdminRequirement>
{
	/// <inheritdoc />
	protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
		AdminRequirement requirement)
	{
		Author author;
		try
		{
			author = await requestContext.RequireAuthorAsync(context.User, db);
		}
		catch (InvalidCredentialException e)
		{
			logger.LogWarning(e, "Denied admin access to a caller which could not be identified");

			return;
		}

		if (author.Role != Role.Admin)
		{
			logger.LogWarning("Denied admin access to {Username} (author {AuthorId}) holding role {Role}",
				author.UserName, author.Id, author.Role);

			return;
		}

		logger.LogInformation("Granted admin access to {Username} (author {AuthorId})", author.UserName, author.Id);

		context.Succeed(requirement);
	}
}
