using Microsoft.EntityFrameworkCore;
using UfgPay.CampusService.Application.Interfaces;
using UfgPay.CampusService.Application.Services;
using UfgPay.CampusService.Domain.Interfaces;
using UfgPay.CampusService.Grpc;
using UfgPay.CampusService.Infrastructure.Data;
using UfgPay.CampusService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configuração DbContext PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Port=5435;Database=ufg_campus_db;Username=postgres;Password=postgrespassword";

builder.Services.AddDbContext<CampusDbContext>(options =>
    options.UseNpgsql(connectionString));

// Injeção de Dependências seguindo SOLID (DIP / SRP)
builder.Services.AddScoped<ICampusRepository, CampusRepository>();
builder.Services.AddScoped<ICampusService, CampusService>();

// gRPC
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<CampusGrpcServiceImpl>();

app.MapGet("/", () => "[UFG Pay - CampusService] Serviço gRPC de Catálogo e Vouchers ativo e operacional.");

app.Run();
