using BLL;
using CCL;
using DAL;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder();

var connection = builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddDatabase(connection);

builder.Services.AddCustumizableServices();
builder.Services.AddServices();
builder.Services.AddControllersLogic();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

        if (!dbContext.IsInitialized())
            ApplicationDbInitializer.InitializeRoles(dbContext);
    }
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();