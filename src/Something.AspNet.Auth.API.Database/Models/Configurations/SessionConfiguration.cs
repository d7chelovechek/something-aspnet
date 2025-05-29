using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Something.AspNet.Auth.API.Database.Models.Configurations;

internal class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .IsRequired();
    }
}