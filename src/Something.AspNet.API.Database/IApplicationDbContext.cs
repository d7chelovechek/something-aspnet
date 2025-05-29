using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Something.AspNet.API.Database.Models;
using System.Data;

namespace Something.AspNet.API.Database;

public interface IApplicationDbContext
{
    public DbSet<User> Users { get; }

    public DbSet<Session> Sessions { get; }

    public DbSet<OutboxEvent> OutboxEvents { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    public Task MigrateDatabaseAsync(CancellationToken cancellationToken);

    public Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken);
}