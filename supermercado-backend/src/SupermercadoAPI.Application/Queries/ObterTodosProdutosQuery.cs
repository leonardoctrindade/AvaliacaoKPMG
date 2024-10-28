using MediatR;
using SupermercadoAPI.Application.DTOs;

namespace SupermercadoAPI.Application.Queries
{
    public class ObterTodosProdutosQuery : IRequest<IEnumerable<ProdutoDTO>>
    {
    }
}
