using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Persistence; // AddPersistenceServices uzantýsýný getirir
using Core.Security.JWT;
using Application.Features.Auths.Commands.SendVerificationCode;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

var builder = WebApplication.CreateBuilder(args);

// 1) Persistence (DbContext, Repos, MailService, JwtHelper, vs.)
builder.Services.AddPersistenceServices(builder.Configuration);

// 2) MediatR — tüm handler’larý tarayacak
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
builder.Services.AddSwaggerGen();
builder.Services.AddCors(p =>
{
    p.AddPolicy("AllowAll", x => x
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
