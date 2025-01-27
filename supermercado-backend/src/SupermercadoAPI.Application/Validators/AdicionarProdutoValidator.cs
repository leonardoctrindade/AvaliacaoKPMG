using FluentValidation;
using SupermercadoAPI.Application.Commands;

namespace SupermercadoAPI.Application.Validators
{
    public class AdicionarProdutoValidator : AbstractValidator<AdicionarProdutoCommand>
    {
        public AdicionarProdutoValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("Nome � obrigat�rio")
                .MaximumLength(100)
                .WithMessage("Nome n�o pode ter mais que 100 caracteres");

            RuleFor(x => x.Setor)
                .NotEmpty()
                .WithMessage("Setor � obrigat�rio");

            RuleFor(x => x.Preco)
                .GreaterThan(0)
                .WithMessage("Pre�o deve ser maior que zero");
        }
    }
}
