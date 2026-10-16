export interface CampusServiceItem {
  id: string;
  estabelecimentoId: string;
  nome: string;
  precoCentavos: number;
  precoReais: number;
}

export interface CheckoutRequestPayload {
  servicoId: string;
  valorCentavos: number;
}

export interface CheckoutResponsePayload {
  sucesso: boolean;
  pedidoId?: string;
  codigoVoucher?: string;
  novoSaldoCentavos?: number;
  novoSaldoReais?: number;
  mensagem?: string;
}
