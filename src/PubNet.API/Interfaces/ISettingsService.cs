using PubNet.API.Configuration;
using PubNet.API.DTO.Settings;

namespace PubNet.API.Interfaces;

public interface ISettingsService
{
	/// <summary>
	/// Returns the declared settings together with their currently effective values.
	/// </summary>
	IReadOnlyList<SettingDescriptorDto> GetAll();

	/// <summary>
	/// Persists the given settings and makes them effective.
	/// </summary>
	/// <remarks>
	/// A <c>null</c> value removes the override, falling back to the value from the configuration files.
	/// When called inside a transaction, the caller is responsible for calling
	/// <see cref="ReloadConfiguration"/> after committing.
	/// </remarks>
	/// <exception cref="Services.UnknownSettingException">A key is not declared in the settings registry.</exception>
	/// <exception cref="Services.InvalidSettingValueException">A value does not match its declared kind.</exception>
	Task ApplyAsync(IReadOnlyDictionary<string, string?> values, CancellationToken cancellationToken = default);

	/// <summary>
	/// Re-reads all configuration sources, making persisted settings visible to
	/// <see cref="Microsoft.Extensions.Options.IOptionsMonitor{T}"/> consumers.
	/// </summary>
	void ReloadConfiguration();
}
