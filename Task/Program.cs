using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task.Context;
using Task.CustomDescriber;
using Task.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<dbContext>(opt =>
{
    opt.UseSqlServer("server=DESKTOP-M783BDP\\SQLEXPRESS; database=IdentityDb; integrated security=true;");
});
builder.Services.AddIdentity<AppUser, AppRole>(opt =>
{
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequiredLength = 1;
    opt.Password.RequireNonAlphanumeric = false;
    opt.SignIn.RequireConfirmedEmail = true;
    opt.Lockout.MaxFailedAccessAttempts = 3;
}).AddErrorDescriber<CustomErrorDescriber>().AddEntityFrameworkStores<dbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.Cookie.HttpOnly = true;
    opt.Cookie.SameSite = SameSiteMode.Strict;
    opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    opt.Cookie.Name = "identityCookie";
    opt.ExpireTimeSpan = TimeSpan.FromDays(25);
    opt.LoginPath = new PathString("/Home/SignIn");
    opt.AccessDeniedPath = new PathString("/Home/AccessDenied");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=SignIn}/{id?}");
app.MapDefaultControllerRoute();
app.Run();
