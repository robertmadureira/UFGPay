using FluentValidation;
using UfgPay.Gateway.Application.DTOs;

namespace UfgPay.Gateway.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Matricula)
            .NotEmpty().WithMessage("A matrícula acadêmica é obrigatória.")
            .MinimumLength(5).WithMessage("A matrícula deve conter ao menos 5 caracteres.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.");
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Matricula)
            .NotEmpty().WithMessage("A matrícula é obrigatória.")
            .MaximumLength(20).WithMessage("A matrícula deve ter no máximo 20 caracteres.");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome completo é obrigatório.")
            .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres.")
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("Formato de e-mail inválido.")
            .Must(email => email.EndsWith("@ufg.br", StringComparison.OrdinalIgnoreCase) || email.EndsWith("@discente.ufg.br", StringComparison.OrdinalIgnoreCase))
            .WithMessage("O e-mail deve ser institucional da UFG (@ufg.br ou @discente.ufg.br).");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve possuir ao menos 6 caracteres.");

        RuleFor(x => x.TipoVinculo)
            .NotEmpty().WithMessage("O tipo de vínculo é obrigatório.")
            .Must(v => new[] { "discente", "docente", "tecnico", "terceirizado" }.Contains(v.ToLowerInvariant()))
            .WithMessage("O vínculo deve ser: discente, docente, tecnico ou terceirizado.");
    }
}

public class TopupPixRequestValidator : AbstractValidator<TopupPixRequest>
{
    public TopupPixRequestValidator()
    {
        RuleFor(x => x.ValorCentavos)
            .GreaterThan(0).WithMessage("O valor da recarga deve ser maior que zero (mínimo de R$ 0,01).");
    }
}

public class CheckoutRequestValidator : AbstractValidator<CheckoutRequest>
{
    public CheckoutRequestValidator()
    {
        RuleFor(x => x.ServicoId)
            .NotEmpty().WithMessage("O identificador do serviço é obrigatório.")
            .Must(id => Guid.TryParse(id, out _)).WithMessage("O identificador do serviço deve ser um GUID válido.");

        RuleFor(x => x.ValorCentavos)
            .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");
    }
}
