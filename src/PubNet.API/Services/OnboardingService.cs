using Microsoft.EntityFrameworkCore;
using PubNet.API.DTO;
using PubNet.API.Interfaces;
using PubNet.Database;
using PubNet.Database.Models;

namespace PubNet.API.Services;

public class OnboardingService(PubNetContext db, IAuthorRegistrationService registration) : IOnboardingService
{
	/// <summary>
	/// Marks onboarding as completed. Being the primary key of the settings table, inserting it also serializes
	/// concurrent attempts to complete onboarding: the loser fails to commit and rolls back.
	/// </summary>
	public const string CompletedAtSettingKey = "Onboarding:CompletedAt";

	/// <inheritdoc />
	public async Task<bool> IsPendingAsync(CancellationToken cancellationToken = default)
	{
		if (await db.Settings.AnyAsync(s => s.Key == CompletedAtSettingKey, cancellationToken))
			return false;

		// installations upgrading from a version without onboarding never completed it explicitly, but the
		// migration promoted their oldest account to admin. Do not send them through the wizard.
		return !await db.Authors.AnyAsync(a => a.Role == Role.Admin, cancellationToken);
	}

	/// <inheritdoc />
	public async Task<Author> CompleteAsync(RegisterRequest request, CancellationToken cancellationToken = default)
	{
		await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

		if (!await IsPendingAsync(cancellationToken))
			throw new OnboardingNotPendingException();

		var author = await registration.RegisterAsync(request, Role.Admin, cancellationToken);

		db.Settings.Add(new()
		{
			Key = CompletedAtSettingKey,
			Value = DateTimeOffset.UtcNow.ToString("O"),
			UpdatedAtUtc = DateTimeOffset.UtcNow,
		});

		await db.SaveChangesAsync(cancellationToken);

		await transaction.CommitAsync(cancellationToken);

		return author;
	}
}
