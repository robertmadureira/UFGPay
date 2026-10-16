namespace UfgPay.WalletService.Application.DTOs;

public record BalanceResultDto(
    Guid UserId,
    long SaldoCentavos,
    bool Bloqueada
);

public record OperationResultDto(
    bool Sucesso,
    string Mensagem,
    long NovoSaldoCentavos,
    Guid? TransacaoId = null
);
