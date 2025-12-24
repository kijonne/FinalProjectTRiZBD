using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OnlineShoeShopLibrary.Services;
using OnlineShoeStoreLibrary.Contexts;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<OnlineShoeStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AuthService>();

// ј”“≈Ќ“»‘» ј÷»я Ч Ё“ќ  Ћё„≈¬ќ… ЅЋќ 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;

        // Ёти 4 строки Ч об€зательно дл€ записи куки в .NET 9
        options.Cookie.Name = "OnlineShoeStore.AuthCookie";  // явное им€ Ч заставл€ет работать
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;  // Ѕез этого кука может не записатьс€
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();