using ETICARET.Business.Abstract;
using ETICARET.Business.Concrete;
using ETICARET.DataAccess.Abstract;
using ETICARET.DataAccess.Concrete.EfCore;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 📄 Razor Pages desteği ekle
builder.Services.AddRazorPages();

// 🗄️ Identity veritabanı bağlantısı ve kullanıcı yönetimi
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"))
);

// 👤 Identity servislerini yapılandır
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

// ⚠️ PROBLEM: Bu yaklaşım önerilmez - Service Provider erken oluşturulmamalı
var userManager = builder.Services.BuildServiceProvider().GetService<UserManager<ApplicationUser>>();
var roleManager = builder.Services.BuildServiceProvider().GetService<RoleManager<IdentityRole>>();

// 🔐 Identity seçeneklerini yapılandır
builder.Services.Configure<IdentityOptions>(options =>
{
    // Şifre gereksinimleri
    options.Password.RequireNonAlphanumeric = true;  // Özel karakter zorunlu
    options.Password.RequireDigit = true;            // Rakam zorunlu
    options.Password.RequireLowercase = true;        // Küçük harf zorunlu
    options.Password.RequireUppercase = true;        // Büyük harf zorunlu
    options.Password.RequiredLength = 6;             // Minimum uzunluk

    // Hesap kilitleme ayarları
    options.Lockout.MaxFailedAccessAttempts = 5;                    // 5 başarısız deneme
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // 5 dakika kilitleme
    options.Lockout.AllowedForNewUsers = true;                      // Yeni kullanıcılar için aktif

    // Kullanıcı ayarları
    options.User.RequireUniqueEmail = true;          // Benzersiz email zorunlu
    options.SignIn.RequireConfirmedEmail = true;     // Email doğrulama zorunlu
    options.SignIn.RequireConfirmedPhoneNumber = false; // Telefon doğrulama opsiyonel
});

// 🍪 Cookie yapılandırması
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";           // Giriş sayfası
    options.LogoutPath = "/account/logout";         // Çıkış sayfası
    options.AccessDeniedPath = "/account/accessdenied"; // Erişim reddedildi sayfası
    options.SlidingExpiration = true;               // Sliding expiration aktif
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // 60 dakika süre
    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,                            // XSS koruması
        Name = "ETICARET.Security.Cookie",          // Cookie adı
        SameSite = SameSiteMode.Strict             // CSRF koruması
    };
});

// 🏢 Business ve DataAccess katmanları DI kaydı
builder.Services.AddScoped<IProductDal, EfCoreProductDal>();
builder.Services.AddScoped<IProductService, ProductManager>();
builder.Services.AddScoped<ICategoryDal, EfCoreCategoryDal>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<ICommentDal, EfCoreCommentDal>();
builder.Services.AddScoped<ICommentService, CommentManager>();
builder.Services.AddScoped<ICartDal, EfCoreCartDal>();
builder.Services.AddScoped<ICartService, CartManager>();
builder.Services.AddScoped<IOrderDal, EfCoreOrderDal>();
builder.Services.AddScoped<IOrderService, OrderManager>();

// 🎭 MVC desteği ekle
builder.Services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest);

var app = builder.Build();

// 🔧 HTTP request pipeline yapılandırması
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// 🌱 Veritabanı seed işlemi (Ürün, Kategori, Ürün-Kategori ilişkileri)
SeedDatabase.Seed();

app.UseStaticFiles();                    // Statik dosyalar
app.CustomStaticFiles();                 // node_modules => modules (Custom middleware)
app.UseHttpsRedirection();               // HTTPS yönlendirme
app.UseAuthentication();                 // Kimlik doğrulama
app.UseAuthorization();                  // Yetkilendirme
app.UseRouting();                        // Routing

// 🛣️ Endpoint yapılandırması
app.UseEndpoints(endpoints =>
{
    // Ana route
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");

    // Ürün kategorisi route
    endpoints.MapControllerRoute(
        name: "products",
        pattern: "products/{category}",
        defaults: new { controller = "Shop", action = "List" }
    );

    // Admin ürün listesi
    endpoints.MapControllerRoute(
        name: "adminProducts",
        pattern: "admin/products",
        defaults: new { controller = "Admin", action = "ProductList" }
    );

    // Admin ürün düzenleme
    endpoints.MapControllerRoute(
        name: "adminProductEdit", // ⚠️ Aynı isim kullanılmamalı
        pattern: "admin/products/{id}",
        defaults: new { controller = "Admin", action = "EditProduct" }
    );

    // Admin kategori listesi
    endpoints.MapControllerRoute(
        name: "adminCategories", // ⚠️ İsim düzeltilmeli
        pattern: "admin/category",
        defaults: new { controller = "Admin", action = "CategoryList" }
    );

    // Admin kategori düzenleme
    endpoints.MapControllerRoute(
        name: "adminCategoryEdit", // ⚠️ İsim düzeltilmeli
        pattern: "admin/categories/{id}",
        defaults: new { controller = "Admin", action = "EditCategory" }
    );

    // Sepet
    endpoints.MapControllerRoute(
        name: "cart",
        pattern: "cart",
        defaults: new { controller = "Cart", action = "Index" }
    );

    // Ödeme
    endpoints.MapControllerRoute(
        name: "checkout",
        pattern: "checkout",
        defaults: new { controller = "Cart", action = "Checkout" }
    );

    // Siparişler
    endpoints.MapControllerRoute(
        name: "orders",
        pattern: "orders",
        defaults: new { controller = "Cart", action = "GetOrders" }
    );
});

// 👥 Identity seed işlemi (Kullanıcı ve rol oluşturma)
SeedIdentity.Seed(userManager, roleManager, app.Configuration).Wait();

app.Run();
