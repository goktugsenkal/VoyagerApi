using System.Globalization;
using System.Text;
using Api.Chat.Hubs;
using Api.Middlewares;
using Api.Misc;
using Core.Constants;
using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Swashbuckle.AspNetCore.SwaggerGen;
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
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Voyager API", Version = VersionInfo.ApiVersion });

        c.DocumentFilter<AddServersDocumentFilter>();
        
        // Add security definition for Bearer authentication
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer", 
            BearerFormat = "JWT", 
            In = ParameterLocation.Header,
            Description = "Paste your token below."
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
                Array.Empty<string>()
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
            
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    var accessToken = ctx.Request.Query["access_token"];
                    var path = ctx.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                    {
                        ctx.Token = accessToken;
                    }
                    return Task.CompletedTask;
                },
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
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IS3Service, S3Service>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserChangeLogRepository, UserChangeLogRepository>();
    builder.Services.AddScoped<IPasswordHasher<VoyagerUser>, PasswordHasher<VoyagerUser>>();
    builder.Services.AddScoped<ISearchRepository, SearchRepository>();
    builder.Services.AddScoped<ISearchService, SearchService>();
    
    builder.Services.AddScoped<IChatService, ChatService>();
    builder.Services.AddSignalR();

    // builder.WebHost.ConfigureKestrel((opt) =>
    // {
    //     opt.ListenAnyIP(5002, options =>
    //     {
    //         options.Protocols = HttpProtocols.Http1AndHttp2;
    //     });
    //     opt.ListenAnyIP(5003, options =>
    //     {
    //         options.UseHttps();
    //     });
    // });

    
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowMySite", policy =>
        {
            policy.WithOrigins(builder.Environment.IsDevelopment() ? "*": "https://voyagerapi.com.tr", "http://localhost:1337")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

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
    
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });


    app.UseHttpsRedirection();

    app.UseMiddleware<ErrorHandlingMiddleware>();
    app.UseMiddleware<UserLoggingMiddleware>();

    app.UseAuthorization();

    app.MapControllers();

    app.MapHub<ChatHub>("/chatHub")
        .RequireAuthorization();

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        app.Logger.LogInformation("Swagger URL: http://localhost:5000/swagger/index.html");
        app.Logger.LogInformation("Swagger URL: https://localhost:5001/swagger/index.html");
    });
    
    app.UseCors("AllowMySite");

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Unhandled exception occurred. Application terminated.");
    throw;
}
finally
{
    LogManager.Shutdown();
}
