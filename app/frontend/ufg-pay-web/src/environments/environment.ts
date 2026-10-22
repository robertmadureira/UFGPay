function getBaseApiUrl(): string {
  if (typeof window !== 'undefined') {
    // Se estiver rodando na Google Cloud CDN pública
    if (window.location.hostname === '136.68.133.89') {
      return 'http://34.36.247.109';
    }
    // Se estiver rodando em desenvolvimento local
    if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
      return 'http://localhost:5050';
    }
  }
  // Padrão de produção no Google Cloud
  return 'http://34.36.247.109';
}

export const environment = {
  production: true,
  apiUrl: getBaseApiUrl()
};
