using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Something.AspNet.Analytics.API.Database.Models;
using System.Data;

namespace Something.AspNet.Analytics.API.Database;

public interface IApplicationDbContext
{
    public DbSet<SessionUpdate> SessionsUpdates { get; }

    public DbSet<OutboxEvent> OutboxEvents { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    public Task MigrateDatabaseAsync(CancellationToken cancellationToken);

    public Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken);
}