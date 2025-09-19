using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Persistence;
using Core.Security.JWT;
using Application.Features.Auths.Commands.SendVerificationCode;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;
using Microsoft.OpenApi.Models;
using Application.Services.Repositories;
using Persistence.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1) Persistence (DbContext, Repos, MailService, JwtHelper, vs.)
builder.Services.AddPersistenceServices(builder.Configuration);

// --- Repositories ---
builder.Services.AddScoped<IExpenseRepository, EfExpenseRepository>();
builder.Services.AddScoped<IPersonalExpenseRepository, EfPersonalExpenseRepository>();
builder.Services.AddScoped<IShareRepository, EfShareRepository>();
builder.Services.AddScoped<IPaymentRepository, EfPaymentRepository>();
builder.Services.AddScoped<IHouseMemberRepository, EfHouseMemberRepository>();
builder.Services.AddScoped<IInvitationRepository, EfInvitationRepository>();
builder.Services.AddScoped<ILedgerLineRepository, EfLedgerLineRepository>();
builder.Services.AddScoped<IRecurringChargeRepository, EfRecurringChargeRepository>();
builder.Services.AddScoped<IChargeCycleRepository, EfChargeCycleRepository>();

builder.Services.AddHttpContextAccessor();

// 2) MediatR — Application assembly’sini tara (V11 uyumlu kayıt)
builder.Services.AddMediatR(typeof(Application.Features.Auths.Commands.SendVerificationCode.SendVerificationCodeCommand).Assembly);

// 3) AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 4) FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(SendVerificationCodeCommand).Assembly);

// 5) JWT
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
        };
    });

// 6) MVC + JSON + Swagger + CORS
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EvArkadasim API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'ınızı 'Bearer {token}' şeklinde girin."
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
                },
                Scheme = "Bearer",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            Array.Empty<string>()
        }
    });

    c.CustomSchemaIds(t => t.FullName);
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

builder.Services.AddCors(p =>
{
    p.AddPolicy("AllowAll", x => x
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// 7) Kestrel: HTTP 5118 + HTTPS 7118
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5118); // HTTP
    options.ListenAnyIP(7118, listen => listen.UseHttps()); // HTTPS
});

var app = builder.Build();

// 8) Otomatik migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// 9) Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Dikkat: HTTPS yönlendirmesi SADECE prod'da
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
