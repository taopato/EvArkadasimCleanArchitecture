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




var builder = WebApplication.CreateBuilder(args);

// 1) Persistence (DbContext, Repos, MailService, JwtHelper, vs.)
builder.Services.AddPersistenceServices(builder.Configuration);

// ► REPO KAYITLARI
builder.Services.AddScoped<IExpenseRepository, EfExpenseRepository>();
builder.Services.AddScoped<IPersonalExpenseRepository, EfPersonalExpenseRepository>();
builder.Services.AddScoped<IShareRepository, EfShareRepository>();
builder.Services.AddScoped<IPaymentRepository, EfPaymentRepository>();

// 2) MediatR — tüm handler’ları tarayacak
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
});

builder.Services.AddCors(p =>
{
    p.AddPolicy("AllowAll", x => x
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});
///////////////////////////////////
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
