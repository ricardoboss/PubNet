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

		builder.HasOne<Identity>(t => t.Identity)
			.WithMany(i => i.Tokens)
			.HasForeignKey(t => t.IdentityId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);

		builder.Property(t => t.Name)
			.HasMaxLength(100);

		builder.Property(t => t.Scopes)
			.HasMaxLength(2000);

		builder.Property(t => t.Value)
			.HasMaxLength(100);

		// https://stackoverflow.com/a/166157/5107884
		builder.Property(t => t.IpAddress)
			.HasMaxLength(45);

		builder.Property(t => t.DeviceType)
			.HasMaxLength(100);

		builder.Property(t => t.Browser)
			.HasMaxLength(100);

		builder.Property(t => t.Platform)
			.HasMaxLength(100);
	}
}
