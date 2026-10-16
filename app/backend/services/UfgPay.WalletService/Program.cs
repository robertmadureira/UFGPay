using Microsoft.EntityFrameworkCore;
using UfgPay.WalletService.Application.Interfaces;
using UfgPay.WalletService.Application.Services;
using UfgPay.WalletService.Domain.Interfaces;
using UfgPay.WalletService.Grpc;
using UfgPay.WalletService.Infrastructure.Data;
using UfgPay.WalletService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configuração DbContext PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Port=5434;Database=ufg_wallet_db;Username=postgres;Password=postgrespassword";

builder.Services.AddDbContext<WalletDbContext>(options =>
    options.UseNpgsql(connectionString));

// Injeção de Dependências seguindo SOLID (DIP / SRP)
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IWalletService, WalletService>();

// gRPC
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<WalletGrpcServiceImpl>();

app.MapGet("/", () => "[UFG Pay - WalletService] Serviço gRPC de Carteira ativo e operacional.");

app.Run();
