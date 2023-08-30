using DAL.Entities;
using DAL.Entities.Configuration.Context;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    
    public DbSet<Role> Roles => Set<Role>();
        
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    
    public DbSet<Notification> Notifications => Set<Notification>();
    
    public DbSet<Post> Posts => Set<Post>();

    public DbSet<Like> Likes => Set<Like>();
    
    public DbSet<Image> Images => Set<Image>();

    public DbSet<Comment> Comments => Set<Comment>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public bool IsInitialized()
    {
        return Users.Any() || Roles.Any() || Subscriptions.Any() 
            || Notifications.Any() || Posts.Any() || Likes.Any()
            || Images.Any() || Comments.Any();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PostEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new LikeEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CommentEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionEntityTypeConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}