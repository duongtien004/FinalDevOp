using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShoeStoreBackend.Data;
using ShoeStoreBackend.Services;
using ShoeStoreBackend.Services.Implementations;
using ShoeStoreBackend.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========= DATABASE =========
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 25))));

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

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };

        // ⚠️ Cho phép HTTP (không bắt buộc HTTPS)
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization();

// ========= CORS FIX =========
// Cho phép FE gọi API từ cả localhost và IP server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://52.64.231.178:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
            // ⚠️ Không dùng AllowCredentials trừ khi bạn dùng cookie
    });
});

// ========= SWAGGER CONFIG =========
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShoeStore API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme.",
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

// ========= SEED DATABASE =========
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await DbInitializer.InitializeAsync(db);
        Console.WriteLine("✅ Seeding cơ sở dữ liệu thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Lỗi khi seeding cơ sở dữ liệu: " + ex.Message);
    }
}

// ========= TEST MYSQL =========
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        if (await db.Database.CanConnectAsync())
            Console.WriteLine("✅ Kết nối MySQL thành công!");
        else
            Console.WriteLine("⚠️ Không thể kết nối MySQL!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ MySQL Error: " + ex.Message);
    }
}

// ========= MIDDLEWARE =========
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// ⚠️ CORS phải ở trước Auth
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
