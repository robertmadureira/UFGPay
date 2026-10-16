# UFG Pay - Sistema Distribuído de Pagamentos Acadêmicos

Sistema de carteira digital e pagamentos acadêmicos para a **Universidade Federal de Goiás (UFG)**, integrando Restaurante Universitário (RU), reprografia (Xerox), lanchonetes e taxas acadêmicas.

---

## 🏛️ Arquitetura do Sistema

O **UFG Pay** segue os princípios de **Clean Architecture**, **SOLID** e o padrão **Database-per-Service**:
- **Frontend SPA (Angular 19+)**: Estrutura modular desacoplada com componentes separados (`.ts`, `.html`, `.scss`), Angular Signals, RxJS e interceptors de autenticação.
- **API Gateway / BFF (ASP.NET Core .NET 9)**: Validações com FluentValidation, autenticação JWT, cache distribuído com Redis 7.4 e orquestração de chamadas gRPC internas.
- **Microsserviços de Domínio (gRPC .NET 9 + EF Core + PostgreSQL 17)**:
  - `UfgPay.UserService`: Autenticação, dados acadêmicos e identidade (Porta 5433).
  - `UfgPay.WalletService`: Controle transacional de saldo em centavos (`int64`), recarga PIX e débitos (Porta 5434).
  - `UfgPay.CampusService`: Gestão de pontos de consumo, pedidos e emissão de vouchers seguros (Porta 5435).

---

## 📁 Estrutura de Pastas

```text
UFGPay/
├── app/
│   ├── backend/
│   │   ├── gateway/
│   │   │   └── UfgPay.Gateway/          # API Gateway REST + BFF + Redis Cache
│   │   └── services/
│   │       ├── UfgPay.UserService/       # Microsserviço gRPC de Usuários
│   │       ├── UfgPay.WalletService/     # Microsserviço gRPC de Carteira e PIX
│   │       └── UfgPay.CampusService/     # Microsserviço gRPC de Catálogo e Vouchers
│   └── frontend/
│       └── ufg-pay-web/                  # Aplicação Web Angular 19+ (Standalone + Signals)
├── infra/
│   ├── docker-compose.yml                # Orquestração de todos os contêineres
│   └── scripts/                          # DDL dos 3 bancos Postgres e Seed de dados
├── proto/                                # Contratos Protocol Buffers (.proto)
└── docs/
    └── adr/
        └── adr0001.md                    # Registro de Decisão Arquitetural (ADR 0001)
```

---

## 🚀 Como Executar

### Pré-requisitos
- **Docker Desktop** instalado e em execução.
- (Opcional para desenvolvimento local nativo) Node.js 22+ e .NET SDK 9+.

### Inicialização com Docker Compose
Para subir todos os bancos de dados, cache Redis, microsserviços gRPC, API Gateway e Frontend com um único comando:

```bash
docker compose up -d --build
```

### URLs de Acesso:
- **Frontend Web (Angular)**: [http://localhost:4200](http://localhost:4200)
- **API Gateway / Swagger UI**: [http://localhost:5000](http://localhost:5000)
- **UserService (gRPC)**: `http://localhost:5001`
- **WalletService (gRPC)**: `http://localhost:5002`
- **CampusService (gRPC)**: `http://localhost:5003`
- **Redis**: `localhost:6379`
- **PostgreSQL User DB**: `localhost:5433`
- **PostgreSQL Wallet DB**: `localhost:5434`
- **PostgreSQL Campus DB**: `localhost:5435`