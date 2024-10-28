using MediatR;
using SupermercadoAPI.Application.Commands;
using SupermercadoAPI.Domain.Interfaces;

public class AtualizarProdutoCommandHandler : IRequestHandler<AtualizarProdutoCommand, bool>
{
    private readonly IProdutoRepository _repository;

    public AtualizarProdutoCommandHandler(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(AtualizarProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await _repository.ObterPorIdAsync(request.Id);
        if (produto == null) return false;

        produto.AtualizarDados(request.Nome, request.Setor, request.Descricao, request.Preco);
        await _repository.AtualizarAsync(produto);
        return true;
    }
}