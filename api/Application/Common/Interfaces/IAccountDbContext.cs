using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PostService.Application.Common.Interfaces;

public interface IAccountDbContext
{
    DbSet<Account> Accounts { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}