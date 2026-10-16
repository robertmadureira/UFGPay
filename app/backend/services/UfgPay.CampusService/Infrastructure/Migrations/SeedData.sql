-- Dados Iniciais (Seed) Expandidos de Estabelecimentos e Catálogo do Campus UFG
INSERT INTO estabelecimentos (id, nome, categoria, ativo) VALUES
('ru_samambaia', 'Restaurante Universitário - Câmpus Samambaia', 'Alimentação', true),
('ru_colemar', 'Restaurante Universitário - Câmpus Colemar Natal e Silva', 'Alimentação', true),
('reprografia_central', 'Reprografia Central (Xerox & Gráfica)', 'Serviços Gráficos', true),
('lanchonete_inf', 'Lanchonete do Instituto de Informática', 'Alimentação', true),
('biblioteca_central', 'Biblioteca Central Professor Alpheu da Veiga Jardim', 'Serviços Acadêmicos', true),
('prograd_ufg', 'Pró-Reitoria de Graduação (PROGRAD)', 'Taxas e Documentos', true)
ON CONFLICT (id) DO UPDATE SET nome = EXCLUDED.nome, categoria = EXCLUDED.categoria, ativo = EXCLUDED.ativo;

INSERT INTO servicos_campus (id, estabelecimento_id, nome, preco_centavos, ativo) VALUES
-- RU Samambaia
('a0000000-0000-0000-0000-000000000001', 'ru_samambaia', 'Refeição RU - Almoço / Jantar (Estudante)', 400, true),
('a0000000-0000-0000-0000-000000000011', 'ru_samambaia', 'Café da Manhã RU - Estudante', 250, true),
('a0000000-0000-0000-0000-000000000012', 'ru_samambaia', 'Refeição RU - Servidor / Visitante', 1400, true),

-- RU Colemar Natal e Silva
('a0000000-0000-0000-0000-000000000002', 'ru_colemar', 'Refeição RU - Almoço / Jantar (Estudante)', 400, true),
('a0000000-0000-0000-0000-000000000013', 'ru_colemar', 'Café da Manhã RU - Estudante', 250, true),

-- Reprografia e Gráfica
('a0000000-0000-0000-0000-000000000003', 'reprografia_central', 'Impressão / Cópia P&B (Unidade)', 25, true),
('a0000000-0000-0000-0000-000000000004', 'reprografia_central', 'Impressão Colorida (Unidade)', 100, true),
('a0000000-0000-0000-0000-000000000014', 'reprografia_central', 'Encadernação com Espiral e Capa Cristal', 650, true),
('a0000000-0000-0000-0000-000000000015', 'reprografia_central', 'Plotagem de Pôster Científico A1', 2800, true),

-- Lanchonete do INF
('a0000000-0000-0000-0000-000000000005', 'lanchonete_inf', 'Café Expresso 50ml', 350, true),
('a0000000-0000-0000-0000-000000000006', 'lanchonete_inf', 'Salgado Assado Diversos', 700, true),
('a0000000-0000-0000-0000-000000000016', 'lanchonete_inf', 'Pão de Queijo Tradicional Goiano', 450, true),
('a0000000-0000-0000-0000-000000000017', 'lanchonete_inf', 'Água Mineral sem Gás 500ml', 300, true),
('a0000000-0000-0000-0000-000000000018', 'lanchonete_inf', 'Suco de Laranja Natural 300ml', 600, true),

-- Biblioteca Central
('a0000000-0000-0000-0000-000000000019', 'biblioteca_central', 'Multa por Atraso de Devolução (Diária)', 150, true),
('a0000000-0000-0000-0000-000000000020', 'biblioteca_central', 'Aluguel de Armário de Estudos (Semestral)', 3500, true),

-- PROGRAD
('a0000000-0000-0000-0000-000000000021', 'prograd_ufg', '2ª Via de Carteirinha Estudantil (Cartão Físico UFG Pay)', 1500, true),
('a0000000-0000-0000-0000-000000000022', 'prograd_ufg', 'Taxa de Reopção / Mudança de Curso', 5000, true)
ON CONFLICT (id) DO UPDATE SET 
    nome = EXCLUDED.nome, 
    preco_centavos = EXCLUDED.preco_centavos, 
    estabelecimento_id = EXCLUDED.estabelecimento_id, 
    ativo = EXCLUDED.ativo;
