using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PostService.Application.Common.Interfaces;

namespace Infrastructure.Persistence;

public class AccountDbContext : DbContext, IAccountDbContext
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Role).IsRequired();
        });
    }
}
