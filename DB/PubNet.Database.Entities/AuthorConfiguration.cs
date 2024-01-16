using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PubNet.Database.Entities;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
	/// <inheritdoc />
	public void Configure(EntityTypeBuilder<Author> builder)
	{
		builder.HasKey(a => a.Id);

		builder.HasIndex(a => a.UserName)
			.IsUnique();

		builder.Property(a => a.UserName)
			.HasMaxLength(50);
	}
}
