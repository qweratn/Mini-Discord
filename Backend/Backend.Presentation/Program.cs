using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backend.Application;
using Backend.Application.Common.Exceptions;
using Backend.Domain.Common;
using Backend.Infrastructure;
using Backend.Infrastructure.Data;
using Backend.Presentation.Hubs;
using Backend.Presentation.Outbox;
using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.IncludeExceptionDetails = (_, _) => false;

    options.Map<ValidationException>(exception =>
    {
        Dictionary<string, string[]> errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .ToArray());

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Detail = "One or more validation errors occurred.",
            Extensions =
            {
                ["code"] = "validation.failed",
            },
        };
    });

    options.Map<DomainException>(exception =>
        new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Domain rule violation",
            Detail = exception.Message,
            Extensions =
            {
                ["code"] = exception.Code,
            },
        });

    options.Map<NotFoundException>(exception =>
        new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Resource not found",
            Detail = exception.Message,
            Extensions =
            {
                ["code"] = exception.Code,
            },
        });

    options.Map<ForbiddenException>(exception =>
        new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Detail = exception.Message,
            Extensions =
            {
                ["code"] = exception.Code,
            },
        });

    options.Map<ConflictException>(exception =>
        new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Conflict",
            Detail = exception.Message,
            Extensions =
            {
                ["code"] = exception.Code,
            },
        });
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(
                JsonNamingPolicy.CamelCase));
    });
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

string authority = builder.Configuration["Clerk:Authority"] ??
                   throw new InvalidOperationException("Clerk:Authority is not configured.");

const string frontendCors = "FrontendCors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCors, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;

        options.RequireHttpsMetadata = true;
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authority,

            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            // TODO: Validate audience later when we have a frontend app
            ValidateAudience = false,

            ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
            ClockSkew = TimeSpan.FromSeconds(30),

            NameClaimType = "sub",
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                string accessToken = context.Request.Query["access_token"];
                PathString path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs/chat"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Mini-Discord", Version = "v1" });

    options.IncludeXmlComments(
        Assembly.GetExecutingAssembly(),
        includeControllerXmlComments: true);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer",
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = [],
    });
});

builder.Services.AddSignalR();

builder.Services.AddHostedService<OutboxBackgroundService>();

WebApplication app = builder.Build();

await using (AsyncServiceScope scope =
             app.Services.CreateAsyncScope())
{
    ApplicationDbContext dbContext =
        scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(frontendCors);

app.UseAuthentication();
app.UseAuthorization();

app.UseProblemDetails();

app.MapControllers();

app.MapHub<ChatHub>("/hubs/chat");

app.Run();

public partial class Program;
