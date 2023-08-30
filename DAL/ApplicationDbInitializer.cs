using DAL.Consts;

namespace DAL;

public class ApplicationDbInitializer
{
    public static void InitializeRoles(ApplicationDbContext dbContext)
    {
        if (dbContext.IsInitialized())
            throw new ArgumentException("Db is already initialized.");
        
        dbContext.Roles.Add(Roles.User);
        dbContext.SaveChanges();
    }
}