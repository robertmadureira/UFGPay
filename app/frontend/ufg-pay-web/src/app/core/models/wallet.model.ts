export interface WalletBalance {
  userId: string;
  saldoCentavos: number;
  saldoReais: number;
  bloqueada: boolean;
}

export interface TopupPixRequestPayload {
  valorCentavos: number;
  chavePix?: string;
}

export interface TopupPixResponsePayload {
  sucesso: boolean;
  mensagem: string;
  novoSaldoCentavos: number;
  novoSaldoReais: number;
  transacaoId?: string;
}
