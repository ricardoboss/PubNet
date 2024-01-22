using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PubNet.Database.Entities.Auth;

public class TokenConfiguration : IEntityTypeConfiguration<Token>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<Token> builder)
	{
		builder.HasKey(t => t.Id);

		builder.HasIndex(t => t.Value)
			.IsUnique();

		builder.HasIndex(t => new { t.IdentityId, t.Name })
			.IsUnique();

		builder.HasOne<Identity>(t => t.Identity)
			.WithMany(i => i.Tokens)
			.HasForeignKey(t => t.IdentityId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(t => t.Name)
			.HasMaxLength(100);

		builder.Property(t => t.Scopes)
			.HasConversion(
				v => string.Join(',', v),
				v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
			)
			.HasMaxLength(2000);
	}
}
