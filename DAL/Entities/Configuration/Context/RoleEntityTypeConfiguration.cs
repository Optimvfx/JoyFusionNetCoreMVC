using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Entities.Configuration.Context;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public  void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId);
    }
}