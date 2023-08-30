using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

[Table("Images")]
public class Image
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key] public Guid Id { get; set; }
    [Required] public string Url { get; set; }
    
    [Required] public Guid PostId { get; set; }
    public virtual Post Post { get; set; }
}