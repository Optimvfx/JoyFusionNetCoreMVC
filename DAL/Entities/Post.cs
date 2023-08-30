using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DAL.Entities;

[Table("Posts")]
public class Post
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key] public Guid Id { get; set; }
    
    [Required] public string Title { get; set; }
    
    [Required] public string Content { get; set; }
    
    [Required] public DateTime PublishDate { get; set; }

    [Required] public Guid AuthorId { get; set; }
    [Required] public virtual User Author { get; set; }
    
    [Required] public virtual ICollection<Like> Likes { get; set; } = null!;
    [NotNull] public uint LikesCount { get; internal set; } = 0;
    
    [Required] public virtual ICollection<Comment> Comments { get; set; } = null!;
    [NotNull] public uint CommentsCount { get; internal set;} = 0;
    
    [Required] public virtual ICollection<Image> Images { get; set; } = null!;
}