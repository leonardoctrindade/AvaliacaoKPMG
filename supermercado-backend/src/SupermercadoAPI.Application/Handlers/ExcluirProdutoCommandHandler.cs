using MediatR;
using SupermercadoAPI.Application.Commands;
using SupermercadoAPI.Domain.Interfaces;

public class ExcluirProdutoCommandHandler : IRequestHandler<ExcluirProdutoCommand, bool>
{
    private readonly IProdutoRepository _repository;

    public ExcluirProdutoCommandHandler(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ExcluirProdutoCommand request, CancellationToken cancellationToken)
    {
        await _repository.ExcluirAsync(request.Id);
        return true;
    }
}