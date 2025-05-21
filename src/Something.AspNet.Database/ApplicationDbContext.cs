using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Something.AspNet.Database.Models;
using Something.AspNet.Database.Models.Configurations;
using Something.AspNet.Database.Options;

namespace Something.AspNet.Database;

internal class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Session> Sessions { get; set; }

    private readonly DatabaseOptions _databaseOptions;

    public ApplicationDbContext(
        IOptions<DatabaseOptions> databaseOptions,
        DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        _databaseOptions = databaseOptions.Value;
        
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        ChangeTracker.AutoDetectChangesEnabled = false;
    }

    public Task MigrateDatabaseAsync(CancellationToken cancellationToken)
    {
        return Database.MigrateAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        if (_databaseOptions is not null)
        {
            optionsBuilder.UseSqlServer(_databaseOptions.ConnectionString);
        }

        optionsBuilder.ConfigureWarnings(w =>
        {
            w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS);
            w.Ignore(CoreEventId.RowLimitingOperationWithoutOrderByWarning);
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
    }
}