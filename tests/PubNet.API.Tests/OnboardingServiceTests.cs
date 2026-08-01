using PubNet.API.Services;
using PubNet.Database.Models;

namespace PubNet.API.Tests;

public class OnboardingServiceTests
{
	[Test]
	public async Task IsPendingAsync_IsTrue_ForAFreshInstallation()
	{
		using var env = new TestEnvironment();

		Assert.That(await env.Onboarding.IsPendingAsync(), Is.True);
	}

	[Test]
	public async Task IsPendingAsync_IsFalse_WhenAnAdminAlreadyExists()
	{
		// installations upgrading from a version without onboarding get their oldest account promoted by
		// the migration and must not be sent through the wizard
		using var env = new TestEnvironment();

		await env.AddAuthorAsync("existing", Role.Admin);

		Assert.That(await env.Onboarding.IsPendingAsync(), Is.False);
	}

	[Test]
	public async Task IsPendingAsync_IsTrue_WhenOnlyNonAdminsExist()
	{
		using var env = new TestEnvironment();

		await env.AddAuthorAsync("someone", Role.Default);

		Assert.That(await env.Onboarding.IsPendingAsync(), Is.True);
	}

	[Test]
	public async Task CompleteAsync_CreatesAnAdmin()
	{
		using var env = new TestEnvironment();

		var author = await env.Onboarding.CompleteAsync(TestEnvironment.ValidRequest());

		await Assert.MultipleAsync(async () =>
		{
			Assert.That(author.Role, Is.EqualTo(Role.Admin));
			Assert.That(author.PasswordHash, Is.Not.Null);
			Assert.That(await env.GetSettingAsync(OnboardingService.CompletedAtSettingKey), Is.Not.Null);
			Assert.That(await env.Onboarding.IsPendingAsync(), Is.False);
		});
	}

	[Test]
	public async Task CompleteAsync_Throws_WhenOnboardingIsNotPending()
	{
		using var env = new TestEnvironment();

		await env.Onboarding.CompleteAsync(TestEnvironment.ValidRequest());

		var request = TestEnvironment.ValidRequest();
		request.Username = "second";
		request.Email = "second@example.com";

		Assert.ThrowsAsync<OnboardingNotPendingException>(() => env.Onboarding.CompleteAsync(request));
	}

	[Test]
	public async Task IsPendingAsync_StaysFalse_AfterEveryAdminIsRemoved()
	{
		// losing the last admin must not re-open onboarding to anonymous callers
		using var env = new TestEnvironment();

		var author = await env.Onboarding.CompleteAsync(TestEnvironment.ValidRequest());

		env.Db.Authors.Remove(author);
		await env.Db.SaveChangesAsync();

		Assert.That(await env.Onboarding.IsPendingAsync(), Is.False);
	}
}
