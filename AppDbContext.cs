using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
            
    }
}