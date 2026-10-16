using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using UfgPay.Gateway.Application.Interfaces;

namespace UfgPay.Gateway.Infrastructure.Security;

/// <summary>
/// Emissor de JWT assinado com HMAC-SHA256.
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateToken(Guid userId, string matricula, string nome, string email, string tipoVinculo)
    {
        _logger.LogInformation("[JwtService.GenerateToken] Gerando token JWT para UserId: {UserId}", userId);

        var secret = _configuration["Jwt:Secret"] ?? "SuperSecretKeyUfgPaySecurityTokenMustBeAtLeast32BytesLong!";
        var issuer = _configuration["Jwt:Issuer"] ?? "UfgPay.Gateway";
        var audience = _configuration["Jwt:Audience"] ?? "UfgPay.Web";
        var expirationMinutes = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var exp) ? exp : 480;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("matricula", matricula),
            new Claim("nome", nome),
            new Claim("email", email),
            new Claim("tipo_vinculo", tipoVinculo),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
