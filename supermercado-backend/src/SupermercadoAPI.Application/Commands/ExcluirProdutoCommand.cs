using MediatR;

namespace SupermercadoAPI.Application.Commands
{
    public class ExcluirProdutoCommand : IRequest<bool>
    {
        public Guid Id { get; }

        public ExcluirProdutoCommand(Guid id)
        {
            Id = id;
        }
    }
}