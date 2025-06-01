using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Something.AspNet.Analytics.API.Database.Models.Configurations;

internal class SessionUpdateConfiguration : IEntityTypeConfiguration<SessionUpdate>
{
    public void Configure(EntityTypeBuilder<SessionUpdate> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder.HasIndex(s => s.SessionId);
        builder.HasIndex(s => s.UserId);
    }
}