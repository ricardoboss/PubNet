using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO.Authentication;
using PubNet.API.Interfaces;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Services;

/// <summary>
/// Creates author accounts. Shared by registration and the first-time setup, which differ only in the role
/// they hand out and in what they do around it.
/// </summary>
public class AuthorRegistrationService(PubNetContext db, PasswordManager passwordManager)
	: IAuthorRegistrationService
{
	/// <inheritdoc />
	public async Task<Author> RegisterAsync(RegisterRequestDto request, Role role,
		CancellationToken cancellationToken = default)
	{
		if (request.Username is null || request.Name is null || request.Password is null || request.Email is null)
			throw AuthorRegistrationException.MissingValues;

		if (await db.Authors.AnyAsync(a => EF.Functions.ILike(a.UserName, request.Username), cancellationToken))
			throw AuthorRegistrationException.UsernameAlreadyInUse;

		if (await db.Authors.AnyAsync(a => EF.Functions.ILike(a.Email, request.Email), cancellationToken))
			throw AuthorRegistrationException.EmailAlreadyInUse;

		var author = new Author
		{
			UserName = request.Username,
			Email = request.Email,
			Name = request.Name,
			Website = request.Website,
			Inactive = false,
			RegisteredAtUtc = DateTimeOffset.UtcNow,
			Role = role,
			Packages = new List<Package>(),
		};

		author.PasswordHash = await passwordManager.GenerateHashAsync(author, request.Password, cancellationToken);

		db.Authors.Add(author);
		await db.SaveChangesAsync(cancellationToken);

		return author;
	}
}
