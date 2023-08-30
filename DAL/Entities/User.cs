using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using DAL.Entities.Configuration;

namespace DAL.Entities;

[Table("Users")]
public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key] public Guid Id { get; set; }

    [EmailAddress]
    [MaxLength(MailConfiguration.MaxLength)]
    [Required] public string Email { get; set; } = null!;

    [MaxLength(NickConfiguration.MaxLength)]
    [MinLength(NickConfiguration.MinLength)]
    [Required] public string Nick { get; set; } = null!;

    [Required] public string PasswordHash { get; set; } = null!;

    [Required] public DateTime RegistrationDate { get; set; }

    [Required] public Guid RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
    
    [AllowNull] public string? Bio { get; set; } = null;

    [Required] public virtual ICollection<Post> Posts { get; set; } = null!;
    [Required] public virtual ICollection<Like> Likes { get; set; } = null!;
    [Required] public virtual ICollection<Comment> Comments { get; set; } = null!;
    [Required] public virtual ICollection<Post> Favorites { get; set; } = null!;
    [Required] public virtual ICollection<Subscription> Subscriptions { get; set; } = null!;
    [Required] public virtual ICollection<Subscription> Subscribers { get; set; } = null!;
    [Required] public virtual ICollection<Notification> Notifications { get; set; } = null!;
}