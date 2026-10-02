using Microsoft.EntityFrameworkCore;
using PubNet.API.Extensions;
using PubNet.API.Interfaces;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Services;

/// <summary>
/// Changes author roles, keeping at least one admin around. The first-time setup cannot be repeated, so an
/// instance losing its last admin could never be administered again.
/// </summary>
public class AuthorRoleService(PubNetContext db, ILogger<AuthorRoleService> logger) : IAuthorRoleService
{
	/// <inheritdoc />
	public async Task<bool> IsLastAdminAsync(Author author, CancellationToken cancellationToken = default)
	{
		if (!author.IsAdmin)
			return false;

		return await db.Authors.CountAsync(a => a.Role == Role.Admin, cancellationToken) <= 1;
	}

	/// <inheritdoc />
	public async Task SetRoleAsync(Author author, Role role, CancellationToken cancellationToken = default)
	{
		if (author.Role == role)
			return;

		if (role != Role.Admin && await IsLastAdminAsync(author, cancellationToken))
			throw new LastAdminException();

		var previousRole = author.Role;

		author.Role = role;
		await db.SaveChangesAsync(cancellationToken);

		logger.LogWarning("{Username} (author {AuthorId}) changed from {PreviousRole} to {Role}",
			author.UserName, author.Id, previousRole, role);
	}
}
