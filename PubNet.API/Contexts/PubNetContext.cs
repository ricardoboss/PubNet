using Microsoft.EntityFrameworkCore;
using PubNet.API.Models;

namespace PubNet.API.Contexts;

public class PubNetContext : DbContext
{
    public DbSet<Package> Packages { get; set; }

    public DbSet<Author> Authors { get; set; }

    public DbSet<AuthorToken> Tokens { get; set; }

    public DbSet<PendingArchive> PendingArchives { get; set; }

    public PubNetContext(DbContextOptions<PubNetContext> options) : base(options)
    {
    }
}