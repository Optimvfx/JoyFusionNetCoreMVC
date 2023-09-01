using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace DAL.Entities;

[Table("Images")]
public class ImageEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key] public Guid Id { get; set; }
    [Required] public byte[] ImageData { get; set; }
    
    [Required] public Guid PostId { get; set; }
    public virtual Post Post { get; set; }
}