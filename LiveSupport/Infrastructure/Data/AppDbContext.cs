using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Ticket>(e =>
        {
            e.Property(x => x.Title).IsRequired().HasMaxLength(120);
            e.Property(x => x.CustomerEmail).IsRequired();
            e.HasIndex(x => new { x.Status, x.Priority });
        });
    }
}
