using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Data;
using Microsoft.Extensions.FileProviders;
using RealEstate.Infrastructure;
using RealEstate.Infrastructure.Repositories;
using RealEstate.Application.Interfaces;
using RealEstate.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FluentValidation;
using RealEstate.Application.Validators;
using RealEstate.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.IO;
using System;
using RealEstate.Application.Mapping;
using RealEstate.WebApi.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// ------------------------
// Serilog configuration
// ------------------------
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services)
                 .Enrich.FromLogContext();
});

// ------------------------
// Validators
// ------------------------
builder.Services.AddTransient<IValidator<PropertyCreateDto>, PropertyCreateDtoValidator>();
builder.Services.AddTransient<IValidator<IFormFile>, FileUploadValidator>();

// ------------------------
// AutoMapper
// ------------------------
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// ------------------------
// Application services
// ------------------------
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IPropertyImageService, PropertyImageService>();
builder.Services.AddScoped<IPropertyTraceService, PropertyTraceService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ------------------------
// Infrastructure
// ------------------------
builder.Services.AddTransient<IDbConnection>(sp =>
    new SqlConnection(configuration.GetConnectionString("MillionDbConnection")));

builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDapperContext, DapperContext>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
builder.Services.AddScoped<IPropertyTraceRepository, PropertyTraceRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ------------------------
// Health checks
// ------------------------
builder.Services.AddHealthChecks();

// ------------------------
// Controllers & Swagger
// ------------------------
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RealEstate API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.'"
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
            new string[] {}
        }
    });
});
// ------------------------
// JWT Authentication
// ------------------------
var jwt = configuration.GetSection("Jwt");
var envKey = Environment.GetEnvironmentVariable("JWT_SECRET");
if (!string.IsNullOrEmpty(envKey))
{
    builder.Configuration["Jwt:Key"] = envKey;
}

var key = Encoding.UTF8.GetBytes(jwt.GetValue<string>("Key"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwt.GetValue<string>("Issuer"),
        ValidAudience = jwt.GetValue<string>("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// ------------------------
// Middleware pipeline
// ------------------------
app.UseSerilogRequestLogging();

var imagesFolder = configuration.GetValue<string>("FileStorage:ImagesFolder");
Directory.CreateDirectory(imagesFolder);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), imagesFolder)),
    RequestPath = "/images"
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RealEstate API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
