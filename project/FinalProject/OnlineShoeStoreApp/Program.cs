using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OnlineShoeShopLibrary.Services;
using OnlineShoeStoreLibrary.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OnlineShoeStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Путь к странице логина (у тебя Login.cshtml)
        options.AccessDeniedPath = "/AccessDenied"; // Опционально
        options.ExpireTimeSpan = TimeSpan.FromDays(14); // Куки на 14 дней
        options.SlidingExpiration = true;
    });

builder.Services.AddDistributedMemoryCache(); // Кэш для сессий (in-memory)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Время жизни сессии
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
