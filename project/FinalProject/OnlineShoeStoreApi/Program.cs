using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineShoeShopLibrary.Services;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.Options;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddAuthentication();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.secretKey)),
        ValidateLifetime = true,

        ValidateIssuer = true,
        ValidIssuer = AuthOptions.issuer,
        ValidateAudience = true,
        ValidAudience = AuthOptions.audience,

        ClockSkew = TimeSpan.FromMinutes(15),
    };
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Это ключевое — игнорируем циклы в навигационных свойствах EF
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        // Дополнительно — нечувствительность к регистру (чтобы логин принимал любой регистр)
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

        // Максимальная глубина (на всякий случай)
        options.JsonSerializerOptions.MaxDepth = 64;
    });


builder.Services.AddScoped<AuthService>();
builder.Services.AddDbContext<OnlineShoeStoreContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
