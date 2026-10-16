-- Migration Inicial: CampusDbContext (ufg_campus_db)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS estabelecimentos (
    id VARCHAR(50) PRIMARY KEY,
    nome VARCHAR(100) NOT NULL,
    categoria VARCHAR(50) NOT NULL,
    ativo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS servicos_campus (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    estabelecimento_id VARCHAR(50) REFERENCES estabelecimentos(id),
    nome VARCHAR(100) NOT NULL,
    preco_centavos BIGINT NOT NULL,
    ativo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS pedidos_consumo (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    servico_id UUID REFERENCES servicos_campus(id),
    valor_centavos BIGINT NOT NULL,
    status VARCHAR(20) NOT NULL,
    codigo_voucher VARCHAR(64) NOT NULL,
    criado_em TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_pedidos_user_id ON pedidos_consumo(user_id);
CREATE UNIQUE INDEX IF NOT EXISTS idx_pedidos_voucher ON pedidos_consumo(codigo_voucher);
