// EvArkadasim.API/Program.cs

using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Persistence;                // AddPersistenceServices uzantısını getirir
using Core.Security.JWT;
using Application.Features.Auths.Commands.SendVerificationCode;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Microsoft.OpenApi.Models;
using Application.Services.Repositories;
using Persistence.Repositories;
using Application.Features.Houses.Profiles;
using System.Reflection;
using System.Text.Json.Serialization;

// ⚠️ Bu namespace şu an projende yoksa derleme hatası verir, o yüzden yoruma aldım.
// using Application.Features.RecurringCharges.Commands.CreateRecurringCharge;

var builder = WebApplication.CreateBuilder(args);

// 1) Persistence (DbContext, Repos, MailService, JwtHelper, vs.)
builder.Services.AddPersistenceServices(builder.Configuration);

// ► REPO KAYITLARI (mevcut olanlar)
builder.Services.AddScoped<IExpenseRepository, EfExpenseRepository>();
builder.Services.AddScoped<IPersonalExpenseRepository, EfPersonalExpenseRepository>();
builder.Services.AddScoped<IShareRepository, EfShareRepository>();
builder.Services.AddScoped<IPaymentRepository, EfPaymentRepository>();

builder.Services.AddScoped<IRecurringChargeRepository, EfRecurringChargeRepository>();
builder.Services.AddScoped<IChargeCycleRepository, EfChargeCycleRepository>();

// ⭐ LedgerLines için DI kaydı (CreateIrregular / DeleteExpense soft-delete vb. kullanıyor)
builder.Services.AddScoped<ILedgerLineRepository, EfLedgerLineRepository>(); // ⭐ eklendi

// 2) MediatR — Application assembly’sini tara
// (MediatR v11 uyumlu; SendVerificationCodeCommand Application assembly’sinde.)
builder.Services.AddMediatR(typeof(SendVerificationCodeCommand).Assembly);

// 3) AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// 4) FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// 5) JWT
var tokenOptions = builder.Configuration
    .GetSection("TokenOptions")
    .Get<TokenOptions>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
        };
    });

// 6) MVC, Swagger, CORS
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EvArkadasim API", Version = "v1" });

    // JWT Bearer için Swagger ayarları
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Bearer token. \"Bearer {token}\" formatında gönderin."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                },
                Scheme = "Bearer",
                Name   = "Bearer",
                In     = ParameterLocation.Header,
            },
            Array.Empty<string>()
        }
    });

    // Aynı isimli sınıflardan (farklı namespace) doğan schema çakışmalarını engelle
    c.CustomSchemaIds(t => t.FullName);                 // ⭐ eklendi

    // Olası aynı route + aynı HTTP verb çakışmalarında ilkini seç (Swagger JSON üretebilsin)
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); // ⭐ eklendi
});

builder.Services.AddCors(p =>
{
    p.AddPolicy("AllowAll", x => x
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        // enum'ları hem sayı hem string olarak parse et
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // Döngüsel referanslardan kaynaklı Swagger/JSON hatalarını önle
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;            // ⭐ eklendi
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // ⭐ eklendi
    });

///////////////////////////////////
// (Mevcut kayıtların aynen bırakıldı)
builder.Services.AddScoped<IExpenseRepository, EfExpenseRepository>();
builder.Services.AddScoped<IPersonalExpenseRepository, EfPersonalExpenseRepository>();
builder.Services.AddScoped<IShareRepository, EfShareRepository>();
builder.Services.AddScoped<IHouseMemberRepository, EfHouseMemberRepository>();
builder.Services.AddAutoMapper(typeof(Program)); // Bu varsa sorun değil
builder.Services.AddAutoMapper(typeof(HouseMappingProfile).Assembly);
builder.Services.AddScoped<IInvitationRepository, EfInvitationRepository>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(typeof(HouseMappingProfile).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(Assembly.Load("Application"));
///////////////////////////////////

var app = builder.Build();

// 7) Otomatik migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// 8) Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles(); // wwwroot altını servis eder

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
