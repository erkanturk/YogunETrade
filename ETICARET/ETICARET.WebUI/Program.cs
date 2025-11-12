using ETICARET.WebUI.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"))
);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddDefaultTokenProviders();
var userManager = builder.Services.BuildServiceProvider().GetService<UserManager<ApplicationUser>>();
var roleManager = builder.Services.BuildServiceProvider().GetService<RoleManager<IdentityRole>>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric=true;
    options.Password.RequireDigit=true;
    options.Password.RequireLowercase=true;
    options.Password.RequireUppercase=true;
    options.Password.RequiredLength=6;

    options.Lockout.MaxFailedAccessAttempts=5;
    options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(5);
    options.Lockout.AllowedForNewUsers=true;
    options.User.RequireUniqueEmail=true;
    options.SignIn.RequireConfirmedEmail=true;
    options.SignIn.RequireConfirmedPhoneNumber=false;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath="/account/login";
    options.LogoutPath="/account/logout";
    options.AccessDeniedPath="/account/accessdenied";
    options.SlidingExpiration=true;//Eðer kullanýcý aktif ise süre uzasýn
    options.ExpireTimeSpan=TimeSpan.FromMinutes(50);
    options.Cookie=new CookieBuilder()
    {
        HttpOnly=true,
        Name="ETICARET.Securty.Cookie",
        SameSite=SameSiteMode.Strict,//Ayný site için eriþim(CSRF önleme)
    };
});
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
