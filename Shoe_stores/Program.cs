using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShoeStoreBackend.Data;
using ShoeStoreBackend.Services;
using ShoeStoreBackend.Services.Implementations;
using ShoeStoreBackend.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== Configuration helpers =====
// Allow configuration from environment variables (already available via builder.Configuration)
var configuration = builder.Configuration;
var env = builder.Environment;

// Read CORS settings from env or config
// If ALLOW_ALL_CORS=true -> allow any origin (useful for development)
// Otherwise, if ALLOWED_ORIGINS is set (semicolon-separated), we'll allow those origins
var allowAllCors = configuration.GetValue<bool?>("ALLOW_ALL_CORS") ?? false;
var allowedOriginsRaw = configuration.GetValue<string?>("ALLOWED_ORIGINS") ?? configuration.GetValue<string?>("AllowedOrigins");

string corsPolicyName = "DefaultCorsPolicy";

// ========= DATABASE =========
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // You can switch to AutoDetect if using different MySQL providers:
    // options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")));
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 25)));
});

// ========= DEPENDENCY INJECTION =========
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddHttpContextAccessor();

// ========= JWT AUTH =========
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? string.Empty))
        };
    });

builder.Services.AddAuthorization();

// ========= CORS =========
// Provide two modes:
// - Development / quick test: set ALLOW_ALL_CORS=true (this will AllowAnyOrigin/AnyHeader/AnyMethod)
// - Safer: set ALLOWED_ORIGINS (semicolon separated) or configure AllowedOrigins in appsettings
builder.Services.AddCors(options =>
{
    if (allowAllCors)
    {
        options.AddPolicy(corsPolicyName, policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    }
    else if (!string.IsNullOrWhiteSpace(allowedOriginsRaw))
    {
        var origins = allowedOriginsRaw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(o => o.Trim())
                                       .ToArray();

        options.AddPolicy(corsPolicyName, policy =>
        {
            policy.WithOrigins(origins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
            // If you need cookies/credentials, call .AllowCredentials() here,
            // but then you MUST use explicit origins (not AllowAnyOrigin).
        });
    }
    else
    {
        // Default to allowing localhost:3000 for convenience if nothing provided
        options.AddPolicy(corsPolicyName, policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    }
});

// ========= SWAGGER CONFIG =========
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShoeStore API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// ===== Forwarded headers (for proxy / load balancers) =====
// This helps app know original scheme (http/https) and client IP when behind proxy.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    // If you need to restrict known proxies, set KnownProxies or KnownNetworks here
});

// ========= SEED DATABASE =========
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await DbInitializer.InitializeAsync(db);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✅ Seeding cơ sở dữ liệu thành công!");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("❌ Lỗi khi seeding cơ sở dữ liệu: " + ex.Message);
        Console.ResetColor();
    }
}

// ========= TEST KẾT NỐI MYSQL =========
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        if (await db.Database.CanConnectAsync())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Kết nối MySQL thành công!");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠️ Không thể kết nối tới MySQL.");
            Console.ResetColor();
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("❌ Lỗi kết nối MySQL: " + ex.Message);
        Console.ResetColor();
    }
}

// ========= MIDDLEWARE =========
// Show Swagger in Development by default; if you want in Production, enable via config or env var
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShoeStore API V1");
    });
}
else
{
    // If you want Swagger in production (not recommended for public APIs), enable via config:
    var swaggerEnabled = configuration.GetValue<bool?>("EnableSwaggerInProduction") ?? false;
    if (swaggerEnabled)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShoeStore API V1");
        });
    }
}

// If behind a reverse proxy that terminates TLS, you might want to disable HTTPS redirection here.
// Keep it enabled for production where app should force HTTPS.
app.UseHttpsRedirection();

// IMPORTANT: CORS must be used BEFORE Authentication/Authorization if the preflight requests need to be handled.
app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
