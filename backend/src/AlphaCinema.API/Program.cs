using System.Text;
using AlphaCinema.API.Middleware;
using AlphaCinema.API.Validators;
using AlphaCinema.Core.DTOs.Auth;
using AlphaCinema.Core.DTOs.DanhGia;
using AlphaCinema.Core.DTOs.KhuyenMai;
using AlphaCinema.Core.DTOs.Phim;
using AlphaCinema.Core.DTOs.SuatChieu;
using AlphaCinema.Core.DTOs.Ve;
using AlphaCinema.Core.Interfaces;
using AlphaCinema.Infrastructure.Data;
using AlphaCinema.Services.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== DATABASE =====
builder.Services.AddDbContext<AlphaCinemaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== SERVICES (Dependency Injection) =====
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPhimService, PhimService>();
builder.Services.AddScoped<IPhongChieuService, PhongChieuService>();
builder.Services.AddScoped<ISuatChieuService, SuatChieuService>();
builder.Services.AddScoped<IDatVeService, DatVeService>();
builder.Services.AddScoped<IHoaDonService, HoaDonService>();
builder.Services.AddScoped<IKhuyenMaiService, KhuyenMaiService>();
builder.Services.AddScoped<IDanhGiaService, DanhGiaService>();
builder.Services.AddScoped<INguoiDungService, NguoiDungService>();
builder.Services.AddScoped<IThanhVienService, ThanhVienService>();

// ===== FLUENT VALIDATION =====
builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
builder.Services.AddScoped<IValidator<CreatePhimRequest>, CreatePhimRequestValidator>();
builder.Services.AddScoped<IValidator<CreateSuatChieuRequest>, CreateSuatChieuRequestValidator>();
builder.Services.AddScoped<IValidator<DatVeRequest>, DatVeRequestValidator>();
builder.Services.AddScoped<IValidator<CreateDanhGiaRequest>, CreateDanhGiaRequestValidator>();
builder.Services.AddScoped<IValidator<CreateKhuyenMaiRequest>, CreateKhuyenMaiRequestValidator>();

// ===== JWT AUTHENTICATION =====
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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
    });

builder.Services.AddAuthorization();

// ===== CORS (cho phép Vue.js frontend) =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue", policy =>
        policy.WithOrigins(
            "http://localhost:5173", "http://localhost:5174", "http://localhost:3000",
            "http://127.0.0.1:5173", "http://127.0.0.1:5174", "http://127.0.0.1:3000"
        )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ===== SWAGGER =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Alpha Cinema API",
        Version = "v1",
        Description = "API hệ thống rạp chiếu phim Alpha Cinema"
    });

    // Thêm JWT vào Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization: Bearer {token}",
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// ===== MIDDLEWARE PIPELINE =====
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alpha Cinema API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowVue");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ===== ROOT ROUTE (Tránh lỗi 404 khi truy cập http://localhost:5059/) =====
app.MapGet("/", () => Results.Text("Alpha Cinema API is running. Visit /swagger to see the documentation.", "text/plain", Encoding.UTF8));

// ===== DB MIGRATIONS & SEED DATA =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AlphaCinemaDbContext>();
    try {
        await db.Database.MigrateAsync(); // Create DB and tables from Migrations
    } catch (Exception ex) { 
        Console.WriteLine($"[DB Warning] Migration skipped or failed: {ex.Message}");
    }

    try {
        await DbSeeder.SeedAsync(db);    // Seed default data if empty
    } catch (Exception ex) {
        Console.WriteLine($"[DB Error] Seeding failed: {ex.Message}");
    }
}

app.Run();
