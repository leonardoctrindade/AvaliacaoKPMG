// ObterProdutoPorIdQuery.cs
using MediatR;
using SupermercadoAPI.Application.DTOs;

namespace SupermercadoAPI.Application.Queries
{
    public class ObterProdutoPorIdQuery : IRequest<ProdutoDTO>
    {
        public Guid Id { get; }

        public ObterProdutoPorIdQuery(Guid id)
        {
            Id = id;
        }
    }
}