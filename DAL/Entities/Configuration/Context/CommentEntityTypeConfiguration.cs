using Laraue.EfCoreTriggers.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Entities.Configuration.Context;

public class CommentEntityTypeConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasIndex(p => p.CreationDate);
        
       builder.AfterInsert(trigger => trigger
            .Action(action => action
                .Update<Post>(
                    (tableRefs, post) => post.Id == tableRefs.New.PostId,
                    (tableRefs, value) =>  new Post()
                    {
                        CommentsCount = value.CommentsCount + 1
                    })));
        
        builder.AfterDelete(trigger => trigger
            .Action(action => action
                .Update<Post>(
                    (tableRefs, post) => post.Id == tableRefs.Old.PostId,
                    (tableRefs, value) =>  new Post()
                    {
                        CommentsCount = value.CommentsCount - 1
                    })));
    }
}