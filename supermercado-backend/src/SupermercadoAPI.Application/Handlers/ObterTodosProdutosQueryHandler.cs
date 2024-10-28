using MediatR;
using SupermercadoAPI.Application.DTOs;
using SupermercadoAPI.Application.Queries;
using SupermercadoAPI.Domain.Interfaces;

public class ObterTodosProdutosQueryHandler : IRequestHandler<ObterTodosProdutosQuery, IEnumerable<ProdutoDTO>>
{
    private readonly IProdutoRepository _repository;

    public ObterTodosProdutosQueryHandler(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProdutoDTO>> Handle(ObterTodosProdutosQuery request, CancellationToken cancellationToken)
    {
        var produtos = await _repository.ObterTodosAsync();
        return produtos.Select(p => new ProdutoDTO
        {
            Id = p.Id,
            Nome = p.Nome,
            Setor = p.Setor,
            Descricao = p.Descricao,
            Preco = p.Preco
        });
    }
}