namespace UfgPay.Gateway.Application.Interfaces;

/// <summary>
/// Contrato para emissão e validação de tokens JWT.
/// </summary>
public interface IJwtService
{
    string GenerateToken(Guid userId, string matricula, string nome, string email, string tipoVinculo);
}
