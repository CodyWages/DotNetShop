using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Stripe;
using System.Security.Claims;
using Microsoft.Extension.DependencyInjection;
using Shop.UI.Infrastructure;
using Shop.Domain.Infrastructure;
using Shop.Database;
using Shop.Application.Cart;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Accounts/Login";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
    // options.AddPolicy("Manager", policy => policy.RequireClaim("Role", "Manager"));
    options.AddPolicy("Manager", policy => policy
        .RequireAssertion(context => 
            context.User.HasClaim("Role", "Manager")
            || context.User.HasClaim("Role", "Admin")));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin");
    options.Conventions.AuthorizePage("/Admin/ConfigureUsers", "Admin");
});

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Cart";
    options.Cookie.MaxAge = TimeSpan.FromMinutes(20);
});

builder.Services.AddTransient<IStockManager, StockManager>();
builder.Services.AddScoped<ISessionManager, SessionManager>();

builder.Services.AddApplicationServices();

var app = builder.Build();

try
{
    using(var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        context.Database.EnsureCreated();

        if (!context.Users.Any())
        {
            var adminUser = new IdentityUser()
            {
                UserName = "Admin"
            };

            var managerUser = new IdentityUser()
            {
                UserName = "Manager"
            };

            userManager.CreateAsync(adminUser, "password").GetAwaiter().GetResult();
            userManager.CreateAsync(managerUser, "password").GetAwaiter().GetResult();

            var adminClaim = new Claim("Role", "Admin");
            var managerClaim = new Claim("Role", "Manager");

            userManager.AddClaimAsync(adminUser, adminClaim).GetAwaiter().GetResult();
            userManager.AddClaimAsync(managerUser, managerClaim).GetAwaiter().GetResult();
        }
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapDefaultControllerRoute();
app.Run();