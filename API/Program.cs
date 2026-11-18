using infrastructure.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"]
    ?? throw new InvalidOperationException("JwtSettings:Secret não foi configurado.");
var issuer = jwtSettings["Issuer"]
    ?? throw new InvalidOperationException("JwtSettings:Issuer não foi configurado.");
var audience = jwtSettings["Audience"]
    ?? throw new InvalidOperationException("JwtSettings:Audience não foi configurado.");

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAssertion(context =>
            string.Equals(
                context.User.FindFirst("perfil")?.Value,
                "Admin",
                StringComparison.OrdinalIgnoreCase)));
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            {
                if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                    return false;

                if (uri.Port != 4200)
                    return false;

                var host = uri.Host;
                return host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                    || host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)
                    || host.StartsWith("10.", StringComparison.Ordinal)
                    || host.StartsWith("192.168.", StringComparison.Ordinal)
                    || host.StartsWith("172.16.", StringComparison.Ordinal)
                    || host.StartsWith("172.17.", StringComparison.Ordinal)
                    || host.StartsWith("172.18.", StringComparison.Ordinal)
                    || host.StartsWith("172.19.", StringComparison.Ordinal)
                    || host.StartsWith("172.20.", StringComparison.Ordinal)
                    || host.StartsWith("172.21.", StringComparison.Ordinal)
                    || host.StartsWith("172.22.", StringComparison.Ordinal)
                    || host.StartsWith("172.23.", StringComparison.Ordinal)
                    || host.StartsWith("172.24.", StringComparison.Ordinal)
                    || host.StartsWith("172.25.", StringComparison.Ordinal)
                    || host.StartsWith("172.26.", StringComparison.Ordinal)
                    || host.StartsWith("172.27.", StringComparison.Ordinal)
                    || host.StartsWith("172.28.", StringComparison.Ordinal)
                    || host.StartsWith("172.29.", StringComparison.Ordinal)
                    || host.StartsWith("172.30.", StringComparison.Ordinal)
                    || host.StartsWith("172.31.", StringComparison.Ordinal);
            })
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();
var userPhotosPath = Path.GetFullPath(
    builder.Configuration["FileStorage:UserPhotosPath"] ?? Path.Combine("wwwroot", "uploads", "usuarios"),
    app.Environment.ContentRootPath);
Directory.CreateDirectory(userPhotosPath);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("APP ELETROINOX Obras").WithTheme(ScalarTheme.Mars);

    });
}

app.UseCors("Frontend");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(userPhotosPath),
    RequestPath = "/uploads/usuarios"
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
