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
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 25))
    ));

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
        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 16)
            throw new InvalidOperationException("JWT Key must be at least 16 characters!");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // Cho phép HTTP trong dev, production nên bật HTTPS
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    });

builder.Services.AddAuthorization();

// ========= CORS - Cho phép React Frontend =========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
                    "http://localhost:3000",
                    "http://52.64.231.178:3000",
                    "https://yourdomain.com" // thêm khi deploy thật
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
                // .AllowCredentials(); // chỉ dùng khi cần cookie/session
    });
});

// ========= SWAGGER =========
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShoeStore API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme. Example: \"Bearer {token}\"",
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
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
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
        Console.WriteLine("Seeding cơ sở dữ liệu thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Lỗi khi seeding: {ex.Message}");
    }
}

// ========= MIDDLEWARE - THỨ TỰ RẤT QUAN TRỌNG =========
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShoeStore API V1");
    c.RoutePrefix = string.Empty; // Swagger tại root: https://yourserver/
});

// CORS phải đứng TRƯỚC mọi thứ (trước cả HttpsRedirection)
app.UseCors("AllowReactApp");

// Chỉ bật HTTPS Redirection khi không phải Development
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
// Trong dev: cho phép HTTP hoàn toàn → không bị redirect → CORS hoạt động

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ========= START APP =========
app.Run();