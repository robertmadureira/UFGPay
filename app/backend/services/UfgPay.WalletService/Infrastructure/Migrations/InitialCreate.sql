-- Migration Inicial: WalletDbContext (ufg_wallet_db)
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS contas (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    saldo_centavos BIGINT NOT NULL DEFAULT 0,
    bloqueada BOOLEAN NOT NULL DEFAULT FALSE,
    atualizado_em TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_saldo_positivo CHECK (saldo_centavos >= 0)
);

CREATE TABLE IF NOT EXISTS transacoes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID NOT NULL,
    tipo VARCHAR(20) NOT NULL,
    valor_centavos BIGINT NOT NULL,
    descricao VARCHAR(255) NOT NULL,
    criado_em TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_contas_user_id ON contas(user_id);
CREATE INDEX IF NOT EXISTS idx_transacoes_user_id ON transacoes(user_id);
