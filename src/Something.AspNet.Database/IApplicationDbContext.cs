using Microsoft.EntityFrameworkCore;
using Something.AspNet.Database.Models;

namespace Something.AspNet.Database;

public interface IApplicationDbContext
{
    public DbSet<User> Users { get; }

    public DbSet<Session> Sessions { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    public Task MigrateDatabaseAsync(CancellationToken cancellationToken);
}