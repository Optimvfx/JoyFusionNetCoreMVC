using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DAL.Entities;

[Table("Comments")]
public class Comment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key] public Guid Id { get; set; }
    
    [AllowNull] public string? Content { get; set; } = null;
    
    [Required] public DateTime CreationDate { get; set; }
      
    [Required] public Guid AuthorId { get; set; }
    public virtual User Author { get; set; }
    
    [Required] public Guid PostId { get; set; }
    public virtual Post Post { get; set; }
}