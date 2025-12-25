using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OnlineShoeShopLibrary.Services;
using OnlineShoeStoreLibrary.Contexts;
using OnlineShoeStoreLibrary.Options;
using Scalar.AspNetCore;
using System.Text;

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
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
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
