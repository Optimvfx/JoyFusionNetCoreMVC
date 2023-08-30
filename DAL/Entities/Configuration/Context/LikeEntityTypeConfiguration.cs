using Laraue.EfCoreTriggers.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Entities.Configuration.Context;

public class LikeEntityTypeConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasKey(l => new { l.AuthorId, l.PostId });
        builder.HasIndex(l => new { l.AuthorId, l.PostId });
        
        builder.AfterInsert(trigger => trigger
            .Action(action => action
                .Update<Post>(
                    (tableRefs, post) => post.Id == tableRefs.New.PostId,
                    (tableRefs, value) =>  new Post()
                    {
                        LikesCount = value.LikesCount + 1,
                    })));
        
        builder.AfterDelete(trigger => trigger
            .Action(action => action
                .Update<Post>(
                    (tableRefs, post) => post.Id == tableRefs.Old.PostId,
                    (tableRefs, value) =>  new Post()
                    {
                        LikesCount = value.LikesCount - 1,
                    })));
    }
}