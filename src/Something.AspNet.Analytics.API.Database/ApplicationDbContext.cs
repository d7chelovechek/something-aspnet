using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Something.AspNet.Analytics.API.Database.Models;
using Something.AspNet.Analytics.API.Database.Models.Configurations;
using Something.AspNet.Analytics.API.Database.Options;
using System.Data;

namespace Something.AspNet.Analytics.API.Database;

internal class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<SessionUpdate> SessionsUpdates { get; set; }

    public DbSet<OutboxEvent> OutboxEvents { get; set; }

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

    public Task<IDbContextTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel, 
        CancellationToken cancellationToken)
    {
        return Database.BeginTransactionAsync(isolationLevel, cancellationToken);
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
        modelBuilder.ApplyConfiguration(new SessionUpdateConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxEventConfiguration());
    }
}