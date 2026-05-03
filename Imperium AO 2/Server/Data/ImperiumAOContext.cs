using ImperiumAO.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace ImperiumAO.Server.Data;

public class ImperiumAOContext : DbContext
{
    public ImperiumAOContext(DbContextOptions<ImperiumAOContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Npc> Npcs => Set<Npc>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Account configuration
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Character (base class) configuration
        modelBuilder.Entity<Character>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
        });

        // Player configuration
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasBaseType<Character>();
            entity.HasOne(e => e.Account)
                .WithMany(a => a.Players)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // NPC configuration
        modelBuilder.Entity<Npc>(entity =>
        {
            entity.HasBaseType<Character>();
        });
    }
}
