using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResQLink.Data;
using ResQLink.Models;
using ResQLink.Services;
using Microsoft.AspNetCore.Identity.UI.Services;



var builder = WebApplication.CreateBuilder(args);

// Add DbContext
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));



// Identity & Services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Create default admin
static async Task CreateDefaultAdminAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string adminEmail = "admin@resqlink.com";
    string adminPassword = "Admin@123";
    string adminRole = "Admin";

    if (!await roleManager.RoleExistsAsync(adminRole))
        await roleManager.CreateAsync(new IdentityRole(adminRole));

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var user = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            Name = "Super Admin"
        };

        var result = await userManager.CreateAsync(user, adminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, adminRole);
    }
}

// Seed on app start
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateDefaultAdminAsync(services);
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles(); // Enables wwwroot

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Routing for Areas (e.g., Admin Area)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Account}/{action=Login}/{id?}");

// Default route (Responder login page)
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Responder}/{controller=Account}/{action=Login}/{id?}");


app.Run();
