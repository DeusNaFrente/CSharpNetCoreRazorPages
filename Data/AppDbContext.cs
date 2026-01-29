using ContatosApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ContatosApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Contact> Contacts => Set<Contact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Filtro global de soft delete
        modelBuilder.Entity<Contact>()
            .HasQueryFilter(c => !c.IsDeleted);

        //Indices unicos
        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.Phone)
            .IsUnique();

        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.Email)
            .IsUnique();
    }
}
