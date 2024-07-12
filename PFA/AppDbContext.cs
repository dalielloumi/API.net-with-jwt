using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PFA.Models;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Tiers> Tiers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignore the Tiers entity during migration generation
        modelBuilder.Entity<Tiers>().ToTable("tiers").HasKey(t => t.Cf);
        modelBuilder.Ignore<Tiers>();   // Exclude Tiers from migrations
    }
}

