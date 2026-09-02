using PubNet.SDK.Generated.Models;

namespace PubNet.SDK.Abstractions;

/// <summary>
/// Administers the instance: its runtime-configurable settings and the roles of its authors.
/// All endpoints require an authenticated admin.
/// </summary>
public interface IAdminService
{
	/// <summary>
	/// Lists the configurable settings together with their currently effective values.
	/// </summary>
	/// <exception cref="Exceptions.AuthenticationRequiredException">Not authenticated.</exception>
	/// <exception cref="Exceptions.UnexpectedResponseException">The API answered in a way the SDK does not model.</exception>
	Task<List<SettingDescriptorDto>?> GetSettingsAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Changes the given settings, keyed by setting key. A <see langword="null"/> value removes the
	/// override, falling back to the value from the configuration files.
	/// </summary>
	/// <exception cref="Exceptions.AuthenticationRequiredException">Not authenticated.</exception>
	/// <exception cref="Exceptions.UnknownSettingException">A key is not a configurable setting.</exception>
	/// <exception cref="Exceptions.InvalidSettingValueException">A value failed the API's validation.</exception>
	/// <exception cref="Exceptions.UnexpectedResponseException">The API answered in a way the SDK does not model.</exception>
	Task UpdateSettingsAsync(IReadOnlyDictionary<string, string?> settings,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Lists the authors of this instance, including inactive ones and their roles.
	/// </summary>
	/// <exception cref="Exceptions.AuthenticationRequiredException">Not authenticated.</exception>
	/// <exception cref="Exceptions.UnexpectedResponseException">The API answered in a way the SDK does not model.</exception>
	Task<AdminAuthorsResponseDto?> GetAuthorsAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Sets the given author's role.
	/// </summary>
	/// <returns>The updated author, or <see langword="null"/> if no author has that username.</returns>
	/// <exception cref="Exceptions.AuthenticationRequiredException">Not authenticated.</exception>
	/// <exception cref="Exceptions.LastAdminException">The author is the last administrator of the instance.</exception>
	/// <exception cref="Exceptions.UnexpectedResponseException">The API answered in a way the SDK does not model.</exception>
	Task<AuthorDto?> SetAuthorRoleAsync(string username, Role role, CancellationToken cancellationToken = default);
}
