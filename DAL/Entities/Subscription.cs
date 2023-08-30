using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

[Table("Subscriptions")]
public class Subscription
{
    [Required] public Guid SubscriberId { get; set; }
    public User Subscriber { get; set; }
    
    [Required] public Guid TargetUserId { get; set; }
    public User TargetUser { get; set; }
}