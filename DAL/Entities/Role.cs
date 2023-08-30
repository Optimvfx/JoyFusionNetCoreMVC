using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Entities.Configuration;

namespace DAL.Entities;

[Table("Roles")]
public class Role
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key] public Guid Id { get; set; }
    
    [MaxLength(RoleConfiguration.MaxTitleLength)]
    [MinLength(RoleConfiguration.MinTitleLength)]
    [Required] public string Title { get; set; } = null!;
    
    [Required] public virtual ICollection<User> Users { get; set; } = null!;
}