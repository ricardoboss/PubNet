using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Abstractions;

public interface IAuthorService
{
	/// <summary>
	/// Lists the authors of this instance.
	/// </summary>
	/// <exception cref="Exceptions.AuthenticationRequiredException">Not authenticated.</exception>
	/// <exception cref="Exceptions.UnexpectedResponseException">The API answered in a way the SDK does not model.</exception>
	Task<AuthorsResponseDto?> GetAuthorsAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Looks up a single author.
	/// </summary>
	/// <returns>The author, or <see langword="null"/> if no author has that username.</returns>
	/// <exception cref="Exceptions.AuthenticationRequiredException">Not authenticated.</exception>
	/// <exception cref="Exceptions.UnexpectedResponseException">The API answered in a way the SDK does not model.</exception>
	Task<AuthorDto?> GetAuthorAsync(string username, CancellationToken cancellationToken = default);

	/// <summary>
	/// Updates the given author's profile.
	/// </summary>
	/// <exception cref="Exceptions.AuthenticationRequiredException">Not authenticated.</exception>
	/// <exception cref="Exceptions.UnauthorizedException">The authenticated author is someone else.</exception>
	/// <exception cref="Exceptions.UnexpectedResponseException">The API answered in a way the SDK does not model.</exception>
	Task UpdateAuthorAsync(string username, EditAuthorRequestDto request, CancellationToken cancellationToken = default);

	/// <summary>
	/// Deletes the given author, which requires confirming their password.
	/// </summary>
	/// <exception cref="Exceptions.AuthenticationRequiredException">Not authenticated.</exception>
	/// <exception cref="Exceptions.UnauthorizedException">The authenticated author is someone else.</exception>
	/// <exception cref="Exceptions.InvalidPasswordException">The confirmation password is wrong.</exception>
	/// <exception cref="Exceptions.LastAdminException">The account is the last administrator of the instance.</exception>
	/// <exception cref="Exceptions.UnexpectedResponseException">The API answered in a way the SDK does not model.</exception>
	Task DeleteAuthorAsync(string username, DeleteAuthorRequestDto request, CancellationToken cancellationToken = default);
}
