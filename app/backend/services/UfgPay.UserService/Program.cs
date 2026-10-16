using Microsoft.EntityFrameworkCore;
using UfgPay.UserService.Application.Interfaces;
using UfgPay.UserService.Application.Services;
using UfgPay.UserService.Domain.Interfaces;
using UfgPay.UserService.Grpc;
using UfgPay.UserService.Infrastructure.Data;
using UfgPay.UserService.Infrastructure.Repositories;
using UfgPay.UserService.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Port=5433;Database=ufg_user_db;Username=postgres;Password=postgrespassword";

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(connectionString));

// Injeção de Dependências seguindo SOLID (DIP / SRP)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();

// Serviços gRPC
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<UserGrpcServiceImpl>();

app.MapGet("/", () => "[UFG Pay - UserService] Serviço gRPC ativo e operacional.");

app.Run();
