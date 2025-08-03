using Core.Security.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Persistence;               // PersistenceServiceRegistration
using Microsoft.EntityFrameworkCore;
using Persistence.Security.JWT;  // JwtHelper
using Application;               // ApplicationServiceRegistration (eðer varsa)

var builder = WebApplication.CreateBuilder(args);

// 1. Persistence & Application katmanlarýný kaydet
builder.Services.AddPersistenceServices(builder.Configuration);

// 2. JWT ayarlarýný al
var tokenOptions = builder.Configuration
    .GetSection("TokenOptions")
    .Get<TokenOptions>();

// 3. Authentication
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
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))
        };
    });

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();  // ?? hatasýz çalýþmasý için en azýndan bir kere!
app.UseAuthorization();

app.MapControllers();
app.Run();
