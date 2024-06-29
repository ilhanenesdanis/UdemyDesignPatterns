using BaseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<AppIdentityDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});

builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.User.RequireUniqueEmail = true;

}).AddEntityFrameworkStores<AppIdentityDbContext>();


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AppIdentityDbContext>();

    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    try
    {
        context.Database.Migrate();

        if (!userManager.Users.Any())
        {
            var users = new List<AppUser>
            {
                new AppUser { UserName = "User1", Email = "user1@gmail.com" },
                new AppUser { UserName = "User2", Email = "user2@gmail.com" },
                new AppUser { UserName = "User3", Email = "user3@gmail.com" },
                new AppUser { UserName = "User4", Email = "user4@gmail.com" },
                new AppUser { UserName = "User5", Email = "user5@gmail.com" },
                new AppUser { UserName = "User6", Email = "user6@gmail.com" }
            };

            foreach (var user in users)
            {
                var result = await userManager.CreateAsync(user, "Password12*");
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    Console.WriteLine($"Error creating user {user.UserName}: {errors}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}






// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();