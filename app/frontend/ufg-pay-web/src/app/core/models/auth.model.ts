export interface LoginCredentials {
  matricula: string;
  senha: string;
}

export interface RegisterData {
  matricula: string;
  nome: string;
  email: string;
  senha: string;
  tipoVinculo: string;
}

export interface AuthSession {
  sucesso: boolean;
  token: string;
  userId?: string;
  nome?: string;
  email?: string;
  tipoVinculo?: string;
  mensagem?: string;
}
