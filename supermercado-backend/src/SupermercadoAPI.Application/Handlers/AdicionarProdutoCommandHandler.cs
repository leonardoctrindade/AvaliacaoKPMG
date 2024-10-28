using MediatR;
using SupermercadoAPI.Application.Commands;
using SupermercadoAPI.Domain.Entities;
using SupermercadoAPI.Domain.Interfaces;

namespace SupermercadoAPI.Application.Handlers
{
    public class AdicionarProdutoCommandHandler : IRequestHandler<AdicionarProdutoCommand, Guid>
    {
        private readonly IProdutoRepository _repository;

        public AdicionarProdutoCommandHandler(IProdutoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(AdicionarProdutoCommand request, CancellationToken cancellationToken)
        {
            var produto = new Produto(
                request.Nome,
                request.Setor,
                request.Descricao,
                request.Preco
            );

            return await _repository.AdicionarAsync(produto);
        }
    }
}