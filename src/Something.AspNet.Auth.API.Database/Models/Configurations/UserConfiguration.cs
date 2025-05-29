using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Something.AspNet.Auth.API.Database.Models;

namespace Something.AspNet.Auth.API.Database.Models.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();

        builder.Property(u => u.Name).HasMaxLength(32);
        builder.Property(u => u.PasswordHash).HasMaxLength(128);
    }
}