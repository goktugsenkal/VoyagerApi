using System.Globalization;
using System.Text;
using Api.Middlewares;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    logger.Debug("Starting application");

    var builder = WebApplication.CreateBuilder(args);

    // Add NLog as the logging provider
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddDbContext<DataContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Voyager API", Version = "v0.0.5" });

        // Add security definition for Bearer authentication
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token as follows: `Bearer {your token}`",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });

        // Add security requirement
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
                new string[] { }
            }
        });
    });

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["AppSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["AppSettings:Audience"],
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
                ValidateIssuerSigningKey = true
            };
        });

    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IVoyageRepository, VoyageRepository>();
    builder.Services.AddScoped<IVoyageService, VoyageService>();
    builder.Services.AddScoped<ILikeService, LikeService>();
    builder.Services.AddScoped<IStopRepository, StopRepository>();
    builder.Services.AddScoped<ILikeRepository, LikeRepository>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<IFeedService, FeedService>();

    var app = builder.Build();

    // Set culture info
    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        });
    }

    app.UseHttpsRedirection();

    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseMiddleware<UserLoggingMiddleware>();

    app.UseAuthorization();

    app.MapControllers();

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        app.Logger.LogInformation("Swagger URL: http://localhost:5000/swagger/index.html");
        app.Logger.LogInformation("Swagger URL: https://localhost:5001/swagger/index.html");
    });

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Unhandled exception occurred. Application terminated.");
    throw;
}
finally
{
    LogManager.Shutdown(); // Ensure NLog resources are properly released
}
