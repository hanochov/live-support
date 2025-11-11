using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Agent> Agents => Set<Agent>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Agent>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);
            e.Property(x => x.Email).IsRequired().HasMaxLength(200);
            e.HasIndex(x => x.Email).IsUnique();
        });

        b.Entity<Ticket>(e =>
        {
            e.Property(x => x.Title).IsRequired().HasMaxLength(120);
            e.Property(x => x.CustomerEmail).IsRequired();
            e.HasIndex(x => new { x.Status, x.Priority });
            e.HasOne(x => x.Agent)
             .WithMany(a => a.Tickets)
             .HasForeignKey(x => x.AgentId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
