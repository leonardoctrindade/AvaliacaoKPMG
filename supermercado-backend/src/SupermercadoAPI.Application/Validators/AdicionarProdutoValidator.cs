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
                .WithMessage("Nome é obrigatório")
                .MaximumLength(100)
                .WithMessage("Nome não pode ter mais que 100 caracteres");

            RuleFor(x => x.Setor)
                .NotEmpty()
                .WithMessage("Setor é obrigatório");

            RuleFor(x => x.Preco)
                .GreaterThan(0)
                .WithMessage("Preço deve ser maior que zero");
        }
    }
}
