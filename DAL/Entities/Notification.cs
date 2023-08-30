using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

[Table("Notifications")]
public class Notification
{  
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key] public Guid Id { get; set; }
    [Required] public string Content { get; set; }
    [Required] public DateTime CreationDate { get; set; }
    
    [Required] public Guid UserId { get; set; }
    public virtual User User { get; set; }
}