using SupermercadoAPI.Domain.Entities;

namespace SupermercadoAPI.Domain.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Produto>> ObterTodosAsync();
        Task<Guid> AdicionarAsync(Produto produto);
        Task AtualizarAsync(Produto produto);
        Task ExcluirAsync(Guid id);
    }
}
