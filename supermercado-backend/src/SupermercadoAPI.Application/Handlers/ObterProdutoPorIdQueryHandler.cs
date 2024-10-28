// Na pasta Handlers do projeto Application
using MediatR;
using SupermercadoAPI.Application.DTOs;
using SupermercadoAPI.Application.Queries;
using SupermercadoAPI.Domain.Interfaces;

namespace SupermercadoAPI.Application.Handlers
{
    public class ObterProdutoPorIdQueryHandler : IRequestHandler<ObterProdutoPorIdQuery, ProdutoDTO>
    {
        private readonly IProdutoRepository _repository;

        public ObterProdutoPorIdQueryHandler(IProdutoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProdutoDTO> Handle(ObterProdutoPorIdQuery request, CancellationToken cancellationToken)
        {
            var produto = await _repository.ObterPorIdAsync(request.Id);
            if (produto == null) return null;

            return new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Setor = produto.Setor,
                Descricao = produto.Descricao,
                Preco = produto.Preco
            };
        }
    }
}