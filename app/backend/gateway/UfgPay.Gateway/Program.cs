using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using UfgPay.CampusService.Grpc;
using UfgPay.Gateway.Application.Interfaces;
using UfgPay.Gateway.Application.Validators;
using UfgPay.Gateway.Infrastructure.Cache;
using UfgPay.Gateway.Infrastructure.Security;
using UfgPay.Gateway.Middlewares;
using UfgPay.UserService.Grpc;
using UfgPay.WalletService.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UFG Pay - API Gateway / BFF",
        Version = "v1",
        Description = "API Gateway de orquestração para o ecossistema UFG Pay."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT no formato: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            Array.Empty<string>()
        }
    });
});

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Autenticação JWT
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "SuperSecretKeyUfgPaySecurityTokenMustBeAtLeast32BytesLong!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "UfgPay.Gateway";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "UfgPay.Web";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Redis
var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse(redisConnection, true);
    config.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(config);
});

// Serviços de Aplicação
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<IJwtService, JwtService>();

// Clientes gRPC
var userServiceUrl = builder.Configuration["Services:UserService"] ?? "http://localhost:5001";
var walletServiceUrl = builder.Configuration["Services:WalletService"] ?? "http://localhost:5002";
var campusServiceUrl = builder.Configuration["Services:CampusService"] ?? "http://localhost:5003";

builder.Services.AddGrpcClient<UserGrpcService.UserGrpcServiceClient>(o =>
{
    o.Address = new Uri(userServiceUrl);
});

builder.Services.AddGrpcClient<WalletGrpcService.WalletGrpcServiceClient>(o =>
{
    o.Address = new Uri(walletServiceUrl);
});

builder.Services.AddGrpcClient<CampusGrpcService.CampusGrpcServiceClient>(o =>
{
    o.Address = new Uri(campusServiceUrl);
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UFG Pay BFF v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseMiddleware<JwtAuthMiddleware>();
app.UseAuthorization();

app.MapControllers();

// Health Check Endpoints para o Google Cloud Load Balancer (Ingress)
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));
app.MapGet("/", () => Results.Ok(new { status = "healthy", service = "UfgPay.Gateway", timestamp = DateTime.UtcNow }));

app.Run();
