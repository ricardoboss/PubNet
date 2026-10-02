using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO.Settings;
using PubNet.API.Services;

namespace PubNet.API.Tests;

public class SettingsServiceTests
{
	[Test]
	public void ApplyAsync_Throws_ForAnUndeclaredKey()
	{
		// the registry is what keeps deployment configuration (connection strings, JWT keys) out of reach
		using var env = new TestEnvironment();

		Assert.ThrowsAsync<UnknownSettingException>(
			() => env.Settings.ApplyAsync(new Dictionary<string, string?> { ["Jwt:SecretKey"] = "pwned" }));
	}

	[Test]
	public void ApplyAsync_Throws_ForAValueNotMatchingItsKind()
	{
		using var env = new TestEnvironment();

		Assert.ThrowsAsync<InvalidSettingValueException>(() => env.Settings.ApplyAsync(
			new Dictionary<string, string?> { [RegistrationOptions.OpenRegistrationKey] = "yes please" }));
	}

	[Test]
	public void ApplyAsync_Throws_ForARelativeUrl()
	{
		var registry = new SettingsRegistryBuilder().With(new()
		{
			Key = "Test:Url",
			Group = "Test",
			Label = "Url",
			Kind = SettingKind.Url,
		}).Build();

		using var env = new TestEnvironment(registry);

		Assert.ThrowsAsync<InvalidSettingValueException>(() => env.Settings.ApplyAsync(
			new Dictionary<string, string?> { ["Test:Url"] = "/api/" }));
	}

	[Test]
	public async Task ApplyAsync_DoesNotPartiallyApply_AnInvalidChangeSet()
	{
		using var env = new TestEnvironment(new SettingsRegistryBuilder().WithTextSetting().Build());

		Assert.ThrowsAsync<InvalidSettingValueException>(() => env.Settings.ApplyAsync(
			new Dictionary<string, string?>
			{
				[SettingsRegistryBuilder.TextKey] = "fine",
				[RegistrationOptions.OpenRegistrationKey] = "not a boolean",
			}));

		Assert.That(await env.GetSettingAsync(SettingsRegistryBuilder.TextKey), Is.Null);
	}

	[Test]
	public async Task ApplyAsync_RemovesTheOverride_ForANullValue()
	{
		using var env = new TestEnvironment();

		await env.Settings.ApplyAsync(
			new Dictionary<string, string?> { [RegistrationOptions.OpenRegistrationKey] = "true" });

		Assert.That(await env.GetSettingAsync(RegistrationOptions.OpenRegistrationKey), Is.EqualTo("true"));

		await env.Settings.ApplyAsync(
			new Dictionary<string, string?> { [RegistrationOptions.OpenRegistrationKey] = null });

		Assert.That(await env.Db.Settings.AnyAsync(s => s.Key == RegistrationOptions.OpenRegistrationKey),
			Is.False);
	}

	[Test]
	public async Task ApplyAsync_OverwritesAnExistingValue()
	{
		using var env = new TestEnvironment();

		await env.Settings.ApplyAsync(
			new Dictionary<string, string?> { [RegistrationOptions.OpenRegistrationKey] = "true" });
		await env.Settings.ApplyAsync(
			new Dictionary<string, string?> { [RegistrationOptions.OpenRegistrationKey] = "false" });

		Assert.That(await env.GetSettingAsync(RegistrationOptions.OpenRegistrationKey), Is.EqualTo("false"));
	}

	[Test]
	public async Task ApplyAsync_DoesNotTouchTheOnboardingMarker()
	{
		// the completion marker shares the table but is not a declared setting
		using var env = new TestEnvironment();

		await env.Onboarding.CompleteAsync(TestEnvironment.ValidRequest());

		await env.Settings.ApplyAsync(
			new Dictionary<string, string?> { [RegistrationOptions.OpenRegistrationKey] = "true" });

		Assert.That(await env.GetSettingAsync(OnboardingService.CompletedAtSettingKey), Is.Not.Null);
	}

	[Test]
	public void GetAll_NeverReturnsSecrets()
	{
		using var env = new TestEnvironment(new SettingsRegistryBuilder().WithSecretSetting().Build());

		env.Configuration[SettingsRegistryBuilder.SecretKey] = "hunter2";

		var secret = env.Settings.GetAll().Single(d => d.Key == SettingsRegistryBuilder.SecretKey);

		Assert.That(secret.Value, Is.Null);
	}

	[Test]
	public void GetAll_ReturnsTheEffectiveValue()
	{
		using var env = new TestEnvironment();

		env.Configuration[RegistrationOptions.OpenRegistrationKey] = "true";

		var setting = env.Settings.GetAll().Single(d => d.Key == RegistrationOptions.OpenRegistrationKey);

		Assert.That(setting.Value, Is.EqualTo("true"));
	}
}
