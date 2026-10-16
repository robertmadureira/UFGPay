export interface UserProfile {
  id: string;
  matricula: string;
  nome: string;
  email: string;
  tipoVinculo: 'discente' | 'docente' | 'tecnico' | 'terceirizado';
  ativo: boolean;
}
