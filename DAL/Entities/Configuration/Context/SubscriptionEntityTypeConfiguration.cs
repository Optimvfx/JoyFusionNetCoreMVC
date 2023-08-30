using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Entities.Configuration.Context;

public class SubscriptionEntityTypeConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => new {s.SubscriberId, s.TargetUserId});
        builder.HasAlternateKey(s => s.SubscriberId);
        builder.HasAlternateKey(s => s.TargetUserId);
    }
}