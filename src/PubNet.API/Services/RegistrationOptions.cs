using PubNet.API.Configuration;
using PubNet.API.DTO.Settings;

namespace PubNet.API.Services;

public class RegistrationOptions
{
	public const string OpenRegistrationKey = "OpenRegistration";

	/// <summary>
	/// Whether anyone can create an account.
	/// </summary>
	public bool OpenRegistration { get; set; }

	public static IEnumerable<SettingDescriptor> Descriptors =>
	[
		new()
		{
			Key = OpenRegistrationKey,
			Group = "General",
			Label = "Open registration",
			Description = "Allow anyone to create an account. When disabled, only existing users can sign in.",
			Kind = SettingKind.Boolean,
		},
	];
}
