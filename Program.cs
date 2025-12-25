using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Simple.Data;
using Simple.Models;
using Simple.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý
builder.Services.AddDbContext<SimpleContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. IDENTITY AYARLARI (AppUser KULLANARAK)
// Burasý "Identity.Application" þemasýný oluþturur.
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<SimpleContext>()
    .AddDefaultTokenProviders();

// 3. AUTHENTICATION (JWT) AYARLARI
// Identity'nin cookie sistemi yerine JWT kullanacaðýmýzý belirtiyoruz.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// 4. Servisleri Kaydet (Dependency Injection)
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (Frontend eriþimi için)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins("http://localhost:5173") // Frontend portunu net olarak belirt
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials(); // Bu artýk güvenle çalýþýr
    });
});

var app = builder.Build();

// 5. Seed Data (Admin Kullanýcýsý Oluþturma)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // await SeedData.Initialize(services); // Hata alýrsan burayý yorum satýrý yapýp tekrar açabilirsin
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seed data oluþturulurken hata çýktý.");
    }
}

// 6. Middleware Sýralamasý (Çok Önemli!)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseStaticFiles(); // Resimler için gerekli

app.UseAuthentication(); // Kimlik Kontrolü (Önce bu)
app.UseAuthorization();  // Yetki Kontrolü (Sonra bu)

app.MapControllers();

app.Run();