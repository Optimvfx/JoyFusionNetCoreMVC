using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

[Table("Likes")]
public class Like
{
    [Required] public Guid AuthorId { get; set; }
    public virtual User Author { get; set; }
    
    [Required] public Guid PostId { get; set; }
    public virtual Post Post { get; set; }
}