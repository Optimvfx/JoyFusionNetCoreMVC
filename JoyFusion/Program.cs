using BLL;
using BLL.Services.TimeService;
using CCL;
using DAL;
using DAL.Entities;
using Laraue.EfCoreTriggers.SqlLite.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDatabase(connection);

builder.Services.AddCustumizableServices();
builder.Services.AddServices();
builder.Services.AddControllersLogic();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => 
    {
        options.LoginPath = new PathString("/Auth/Login");
    });

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.HeaderName = "X-CSRF-TOKEN";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

        if (!dbContext.IsInitialized())
            ApplicationDbInitializer.InitializeRoles(dbContext);
    }
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();