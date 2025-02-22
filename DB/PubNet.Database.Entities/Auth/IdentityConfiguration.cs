using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubNet.Auth.Models;

namespace PubNet.Database.Entities.Auth;

public class IdentityConfiguration : IEntityTypeConfiguration<Identity>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<Identity> builder)
	{
		builder.HasKey(i => i.Id);

		builder.HasIndex(i => i.Email)
			.IsUnique();

		builder.HasOne<Author>(i => i.Author)
			.WithOne(a => a.Identity)
			.HasForeignKey<Identity>(i => i.AuthorId)
			.IsRequired()
			.OnDelete(DeleteBehavior.Cascade);

		builder.Navigation(i => i.Author)
			.AutoInclude();

		builder.Property(i => i.Email)
			.HasMaxLength(250);

		builder.Property(i => i.PasswordHash)
			.HasMaxLength(300);

		builder.Property(i => i.Role)
			.HasDefaultValue(Role.Default);
	}
}
